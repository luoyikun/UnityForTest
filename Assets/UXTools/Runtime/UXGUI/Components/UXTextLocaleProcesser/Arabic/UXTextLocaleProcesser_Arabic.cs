using ArabicSupport;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.UXText;

namespace UnityEngine.UI
{
    public class UXTextLocaleProcesser_Arabic : UXTextLocaleProcesser
    {
        [SerializeField] private string m_ReversedFixedText;

        [SerializeField] private bool m_UseTashkeel = false;
        public bool UseTashkeel
        {
            get => m_UseTashkeel;
            set
            {
                if (m_UseTashkeel != value)
                {
                    m_UseTashkeel = value;
                }
            }
        }

        [SerializeField] private bool m_UseHinduNumber = false;
        public bool UseHinduNumber
        {
            get => m_UseHinduNumber;
            set
            {
                if (m_UseHinduNumber != value)
                {
                    m_UseHinduNumber = value;
                }
            }
        }

        public UXTextLocaleProcesser_Arabic(UXText text):base(text)
        {
            LocalizationType = LocalizationTypeDef.arSA;
        }

        public override void ModifyLocaleTextSettings()
        {
            //LocaleText.alignment = TextAnchor.MiddleRight;
        }

        public override string GenLocaleRenderedString(string text)
        {
            OriginString = text;
            SplitAndFixText();
            FillStringInfos();
            string needRenderText = GenFitWidthRenderedText();
            return needRenderText;
        }

        //从m_OrigialText对句子拆分，单行修正阿拉伯语，句子顺序被翻转
        protected void SplitAndFixText()
        {
            if (OriginString.Contains(Environment.NewLine))
                OriginString = OriginString.Replace(Environment.NewLine, "\n");

            List<string> lines = new List<string>(OriginString.Split('\n'));

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = ArabicFixer.Fix(lines[i], m_UseTashkeel, false, m_UseHinduNumber);
            }
            lines.Reverse();

            m_ReversedFixedText = string.Join("\n", lines);
        }

        protected string GenFitWidthRenderedText()
        {
            //保存原有HWrap状态
            bool m_HorizontalWrap = LocaleText.customGenerationSettings.horizontalOverflow == HorizontalWrapMode.Wrap;

            //根据顶点插入换行符，重新生成网格
            if (m_HorizontalWrap)
            {
                LocaleText.customGenerationSettings.horizontalOverflow = HorizontalWrapMode.Overflow;

                LocaleText.cachedTextGenerator.PopulateWithErrors(m_ReversedFixedText, LocaleText.customGenerationSettings, LocaleText.gameObject);

                IList<UIVertex> verts = LocaleText.cachedTextGenerator.verts;
                string wrappedText = ManuallyRTLWrapFromMesh(verts);
                return wrappedText;
            }
            else
            {
                return m_ReversedFixedText;
            }
        }

        #region Info members of text and mesh
        public struct CharMeshInfo
        {
            public int charIdx;
            public float xMin, xMax;
        }

        public struct WordMeshInfo
        {
            //text字符串中的单词信息
            public int startCharIdx;
            public int numOfChars;
            public int endCharIdx { get => startCharIdx + numOfChars - 1; }

            //mesh中的单词信息
            public int startMeshIdx;
            public int endMeshIdx { get => startMeshIdx + numOfChars - 1; }

            public float xMin, xMax;
        }

        public struct LineInfo
        {
            public int startWordIdx;
            public int numOfWords;
            public int startCharIdx;
            public int numofChars;
            public int endWordIdx { get => startWordIdx + numOfWords - 1; }
            public int endCharIdx { get => startCharIdx + numofChars - 1; }
        }

        protected List<CharMeshInfo> m_CharMeshInfos = new List<CharMeshInfo>();
        protected List<WordMeshInfo> m_WordMeshInfos = new List<WordMeshInfo>();
        protected List<LineInfo> m_LineInfos = new List<LineInfo>();

        public int NaturalLineCount { get => m_LineInfos.Count; }
        public int TotalWordCount { get => m_WordMeshInfos.Count; }

        /// <summary>
        /// 拆分文本字符信息 处理每行 每个单词 的起止字符、字符个数信息
        /// </summary>
        protected void FillStringInfos()
        {
            m_WordMeshInfos.Clear();
            m_LineInfos.Clear();

            LineInfo curLine = new LineInfo();
            WordMeshInfo curWordInfo = new WordMeshInfo();

            int totalCharIdx = 0;
            int curWordIdx = 0;
            if (m_ReversedFixedText == null)
                return;
            string[] reversedFixedLines = m_ReversedFixedText.Split('\n');
            for (int lineIdx = 0; lineIdx < reversedFixedLines.Length; lineIdx++)
            {
                curLine.startCharIdx = totalCharIdx;
                curLine.startWordIdx = curWordIdx;
                string[] words = reversedFixedLines[lineIdx].Split(' ');

                curLine.numOfWords = words.Length;
                int localCharIdx = 0;

                for (int wordIdx = 0; wordIdx < words.Length; wordIdx++)
                {
                    string word = words[wordIdx];
                    curWordInfo.startCharIdx = curLine.startCharIdx + localCharIdx;
                    curWordInfo.numOfChars = word.Length;
                    m_WordMeshInfos.Add(curWordInfo);
                    localCharIdx += word.Length + 1;
                }

                curLine.numofChars = localCharIdx - 1;
                m_LineInfos.Add(curLine);

                curWordIdx += words.Length;
                totalCharIdx += localCharIdx;
            }
        }

        /// <summary>
        /// 从顶点数据获取Mesh与Char的映射关系，每4个顶点对应一个CharMesh，获取每个CharMesh的最大和最小的X坐标
        /// </summary>
        /// <param name="verts"></param>
        protected void FillMeshInfos(IList<UIVertex> verts)
        {
            if (verts.Count == 0) return;
            m_CharMeshInfos.Clear();
            if (m_WordMeshInfos.Count == 0) FillStringInfos();
            if (m_WordMeshInfos.Count == 0) return;

            int charIdx = 0, wordIdx = 0;
            while (m_WordMeshInfos[wordIdx].numOfChars == 0) wordIdx++; //跳过开头空格
            charIdx = wordIdx; //起始的charIdx 和 wordIdx是相同的,因为之前跳过的每个空格占一个char
            WordMeshInfo curWordInfo = m_WordMeshInfos[wordIdx];
            for (int meshIdx = 0; meshIdx * 4 < verts.Count; meshIdx++)
            {
                CharMeshInfo curCharMesh;
                curCharMesh.charIdx = charIdx;
                curCharMesh.xMin = verts[meshIdx * 4].position.x;
                curCharMesh.xMax = verts[meshIdx * 4 + 1].position.x;
                m_CharMeshInfos.Add(curCharMesh);

                //在每个词首存入单词信息
                if (charIdx == 0 || m_ReversedFixedText[charIdx - 1] == ' ' || m_ReversedFixedText[charIdx - 1] == '\n')
                {
                    curWordInfo = m_WordMeshInfos[wordIdx];
                    curWordInfo.startMeshIdx = meshIdx;
                    curWordInfo.xMin = curCharMesh.xMin;
                }

                charIdx++;

                //在词尾空格处将单词信息存入列表
                if (charIdx >= m_ReversedFixedText.Length || m_ReversedFixedText[charIdx] == ' ' || m_ReversedFixedText[charIdx] == '\n')
                {
                    curWordInfo.xMax = curCharMesh.xMax;
                    m_WordMeshInfos[wordIdx] = curWordInfo;
                    charIdx++;
                    wordIdx++;
                    while (wordIdx < m_WordMeshInfos.Count && m_WordMeshInfos[wordIdx].numOfChars == 0)
                        wordIdx++; //跳过空词
                }
            }
        }
        #endregion Info members of text and mesh


        /// <summary>
        /// 找到换行处，插入回车符，生成新的字符串
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        string ManuallyRTLWrapFromMesh(IList<UIVertex> verts)
        {
            //获取框的总宽度
            float widthLimit = LocaleText.gameObject.GetComponent<RectTransform>().rect.width;

            FillMeshInfos(verts);

            List<int> charIndicesToInsert = new List<int>();
            foreach (LineInfo line in m_LineInfos)
            {
                float lineXMax = m_WordMeshInfos[line.endWordIdx].xMax;
                for (int wordIdx = line.endWordIdx; wordIdx >= line.startWordIdx; wordIdx--)
                {
                    WordMeshInfo curWordMesh = m_WordMeshInfos[wordIdx];
                    if (lineXMax - curWordMesh.xMin > widthLimit)
                    {
                        charIndicesToInsert.Add(curWordMesh.endCharIdx + 1);
                        lineXMax = curWordMesh.xMax;
                    }
                }
            }

            charIndicesToInsert.Sort();
            if (m_ReversedFixedText == null)
                return "";
            List<char> fixedTextBuffer = new List<char>(m_ReversedFixedText);
            int insertedCount = 0;
            foreach (int insertIdx in charIndicesToInsert)
            {
                fixedTextBuffer.Insert(insertIdx + insertedCount, '\n');
                insertedCount++;
            }
            char[] fixedChars = fixedTextBuffer.ToArray();
            string fixedText = new string(fixedChars);
            List<string> splitLines = new List<string>(fixedText.Split('\n'));
            splitLines.Reverse();
            fixedText = string.Join("\n", splitLines);
            return fixedText;
        }
    }
}
