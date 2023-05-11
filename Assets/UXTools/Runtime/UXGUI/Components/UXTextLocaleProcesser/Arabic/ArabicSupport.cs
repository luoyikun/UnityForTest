//-----------------------------------------------------------------------------
/// <summary>
/// This is an Open Source File Created by: Abdullah Konash. Twitter: @konash
/// This File allow the users to use arabic text in XNA and Unity platform.
/// It flips the characters and replace them with the appropriate ones to connect the letters in the correct way.
/// 
/// The project is available on GitHub here: https://github.com/Konash/arabic-support-unity
/// Unity Asset Store link: https://www.assetstore.unity3d.com/en/#!/content/2674
/// Please help in improving the plugin. 
/// 
/// I would love to see the work you use this plugin for. Send me a copy at: abdullah.konash[at]gmail[dot]com
/// </summary>
/// 
/// <license>
/// MIT License
/// 
/// Copyright(c) 2018
/// Abdullah Konash
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// /// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
/// 
/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.
/// </license>

//-----------------------------------------------------------------------------


#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace ArabicSupport
{

    public class ArabicFixer
    {
        /// <summary>
        /// Fix the specified string.
        /// </summary>
        /// <param name='str'>
        /// String to be fixed.
        /// </param>
        public static string Fix(string str)
        {
            return Fix(str, false, true);
        }

        public static string Fix(string str, bool rtl)
        {
            if (rtl)

            {
                return Fix(str);
            }
            else
            {
                string[] words = str.Split(' ');
                string result = "";
                string arabicToIgnore = "";
                foreach (string word in words)
                {
                    if (char.IsLower(word.ToLower()[word.Length / 2]))
                    {
                        result += Fix(arabicToIgnore) + word + " ";
                        arabicToIgnore = "";
                    }
                    else
                    {
                        arabicToIgnore += word + " ";

                    }
                }
                if (arabicToIgnore != "")
                    result += Fix(arabicToIgnore);

                return result;
            }
        }

        /// <summary>
        /// Fix the specified string with customization options.
        /// </summary>
        /// <param name='str'>
        /// String to be fixed.
        /// </param>
        /// <param name='showTashkeel'>
        /// Show tashkeel.
        /// </param>
        /// <param name='useHinduNumbers'>
        /// Use hindu numbers.
        /// </param>
        public static string Fix(string str, bool showTashkeel, bool useHinduNumbers)
        {
            ArabicFixerTool.showTashkeel = showTashkeel;
            ArabicFixerTool.useHinduNumbers = useHinduNumbers;

            if (str.Contains("\n"))
                str = str.Replace("\n", Environment.NewLine);

            if (str.Contains(Environment.NewLine))
            {
                string[] stringSeparators = new string[] { Environment.NewLine };
                string[] strSplit = str.Split(stringSeparators, StringSplitOptions.None);

                if (strSplit.Length == 0)
                    return ArabicFixerTool.FixLine(str);
                else if (strSplit.Length == 1)
                    return ArabicFixerTool.FixLine(str);
                else
                {
                    string outputString = ArabicFixerTool.FixLine(strSplit[0]);
                    int iteration = 1;
                    if (strSplit.Length > 1)
                    {
                        while (iteration < strSplit.Length)
                        {
                            outputString += Environment.NewLine + ArabicFixerTool.FixLine(strSplit[iteration]);
                            iteration++;
                        }
                    }
                    return outputString;
                }
            }
            else
            {
                return ArabicFixerTool.FixLine(str);
            }

        }

        public static string Fix(string str, bool showTashkeel, bool combineTashkeel, bool useHinduNumbers)
        {
            ArabicFixerTool.combineTashkeel = combineTashkeel;
            return Fix(str, showTashkeel, useHinduNumbers);
        }

    }



    /// <summary>
    /// Arabic Contextual forms General - Unicode
    /// </summary>
    internal enum IsolatedArabicLetters
    {
        Hamza = 0xFE80,
        Alef = 0xFE8D,
        AlefHamza = 0xFE83,
        WawHamza = 0xFE85,
        AlefMaksoor = 0xFE87,
        AlefMaksora = 0xFBFC,
        HamzaNabera = 0xFE89,
        Ba = 0xFE8F,
        Ta = 0xFE95,
        Tha2 = 0xFE99,
        Jeem = 0xFE9D,
        H7aa = 0xFEA1,
        Khaa2 = 0xFEA5,
        Dal = 0xFEA9,
        Thal = 0xFEAB,
        Ra2 = 0xFEAD,
        Zeen = 0xFEAF,
        Seen = 0xFEB1,
        Sheen = 0xFEB5,
        S9a = 0xFEB9,
        Dha = 0xFEBD,
        T6a = 0xFEC1,
        T6ha = 0xFEC5,
        Ain = 0xFEC9,
        Gain = 0xFECD,
        Fa = 0xFED1,
        Gaf = 0xFED5,
        Kaf = 0xFED9,
        Lam = 0xFEDD,
        Meem = 0xFEE1,
        Noon = 0xFEE5,
        Ha = 0xFEE9,
        Waw = 0xFEED,
        Ya = 0xFEF1,
        AlefMad = 0xFE81,
        TaMarboota = 0xFE93,
        PersianPe = 0xFB56,     // Persian Letters;
        PersianChe = 0xFB7A,
        PersianZe = 0xFB8A,
        PersianGaf = 0xFB92,
        PersianGaf2 = 0xFB8E

    }

    /// <summary>
    /// Arabic Contextual forms - Isolated
    /// </summary>
    internal enum GeneralArabicLetters
    {
        Hamza = 0x0621,
        Alef = 0x0627,
        AlefHamza = 0x0623,
        WawHamza = 0x0624,
        AlefMaksoor = 0x0625,
        AlefMagsora = 0x0649,
        HamzaNabera = 0x0626,
        Ba = 0x0628,
        Ta = 0x062A,
        Tha2 = 0x062B,
        Jeem = 0x062C,
        H7aa = 0x062D,
        Khaa2 = 0x062E,
        Dal = 0x062F,
        Thal = 0x0630,
        Ra2 = 0x0631,
        Zeen = 0x0632,
        Seen = 0x0633,
        Sheen = 0x0634,
        S9a = 0x0635,
        Dha = 0x0636,
        T6a = 0x0637,
        T6ha = 0x0638,
        Ain = 0x0639,
        Gain = 0x063A,
        Fa = 0x0641,
        Gaf = 0x0642,
        Kaf = 0x0643,
        Lam = 0x0644,
        Meem = 0x0645,
        Noon = 0x0646,
        Ha = 0x0647,
        Waw = 0x0648,
        Ya = 0x064A,
        AlefMad = 0x0622,
        TaMarboota = 0x0629,
        PersianPe = 0x067E,     // Persian Letters;
        PersianChe = 0x0686,
        PersianZe = 0x0698,
        PersianGaf = 0x06AF,
        PersianGaf2 = 0x06A9

    }

    /// <summary>
    /// Data Structure for conversion
    /// </summary>
    internal class ArabicMapping
    {
        public int from;
        public int to;
        public ArabicMapping(int from, int to)
        {
            this.from = from;
            this.to = to;
        }
    }

    /// <summary>
    /// Sets up and creates the conversion table 
    /// </summary>
    internal class ArabicTable
    {

        private static List<ArabicMapping> mapList;
        private static ArabicTable arabicMapper;

        /// <summary>
        /// Setting up the conversion table
        /// </summary>
        private ArabicTable()
        {
            mapList = new List<ArabicMapping>();



            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Hamza, (int)IsolatedArabicLetters.Hamza));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Alef, (int)IsolatedArabicLetters.Alef));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.AlefHamza, (int)IsolatedArabicLetters.AlefHamza));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.WawHamza, (int)IsolatedArabicLetters.WawHamza));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.AlefMaksoor, (int)IsolatedArabicLetters.AlefMaksoor));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.AlefMagsora, (int)IsolatedArabicLetters.AlefMaksora));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.HamzaNabera, (int)IsolatedArabicLetters.HamzaNabera));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Ba, (int)IsolatedArabicLetters.Ba));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Ta, (int)IsolatedArabicLetters.Ta));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Tha2, (int)IsolatedArabicLetters.Tha2));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Jeem, (int)IsolatedArabicLetters.Jeem));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.H7aa, (int)IsolatedArabicLetters.H7aa));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Khaa2, (int)IsolatedArabicLetters.Khaa2));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Dal, (int)IsolatedArabicLetters.Dal));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Thal, (int)IsolatedArabicLetters.Thal));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Ra2, (int)IsolatedArabicLetters.Ra2));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Zeen, (int)IsolatedArabicLetters.Zeen));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Seen, (int)IsolatedArabicLetters.Seen));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Sheen, (int)IsolatedArabicLetters.Sheen));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.S9a, (int)IsolatedArabicLetters.S9a));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Dha, (int)IsolatedArabicLetters.Dha));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.T6a, (int)IsolatedArabicLetters.T6a));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.T6ha, (int)IsolatedArabicLetters.T6ha));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Ain, (int)IsolatedArabicLetters.Ain));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Gain, (int)IsolatedArabicLetters.Gain));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Fa, (int)IsolatedArabicLetters.Fa));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Gaf, (int)IsolatedArabicLetters.Gaf));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Kaf, (int)IsolatedArabicLetters.Kaf));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Lam, (int)IsolatedArabicLetters.Lam));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Meem, (int)IsolatedArabicLetters.Meem));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Noon, (int)IsolatedArabicLetters.Noon));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Ha, (int)IsolatedArabicLetters.Ha));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Waw, (int)IsolatedArabicLetters.Waw));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.Ya, (int)IsolatedArabicLetters.Ya));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.AlefMad, (int)IsolatedArabicLetters.AlefMad));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.TaMarboota, (int)IsolatedArabicLetters.TaMarboota));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.PersianPe, (int)IsolatedArabicLetters.PersianPe));      // Persian Letters;
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.PersianChe, (int)IsolatedArabicLetters.PersianChe));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.PersianZe, (int)IsolatedArabicLetters.PersianZe));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.PersianGaf, (int)IsolatedArabicLetters.PersianGaf));
            mapList.Add(new ArabicMapping((int)GeneralArabicLetters.PersianGaf2, (int)IsolatedArabicLetters.PersianGaf2));




            //for (int i = 0; i < generalArabic.Length; i++)
            //    mapList.Add(new ArabicMapping((int)generalArabic.GetValue(i), (int)isolatedArabic.GetValue(i)));    // I


        }

        /// <summary>
        /// Singleton design pattern, Get the mapper. If it was not created before, create it.
        /// </summary>
        internal static ArabicTable ArabicMapper
        {
            get
            {
                if (arabicMapper == null)
                    arabicMapper = new ArabicTable();
                return arabicMapper;
            }
        }

        internal int Convert(int toBeConverted)
        {

            foreach (ArabicMapping arabicMap in mapList)
                if (arabicMap.from == toBeConverted)
                {
                    return arabicMap.to;
                }
            return toBeConverted;
        }


    }


    internal class TashkeelLocation
    {
        public char tashkeel;
        public int position;
        public TashkeelLocation(char tashkeel, int position)
        {
            this.tashkeel = tashkeel;
            this.position = position;
        }
    }


    internal class ArabicFixerTool
    {
        internal static bool showTashkeel = true;
        internal static bool combineTashkeel = true;
        internal static bool useHinduNumbers = false;


        internal static string RemoveTashkeel(string str, out List<TashkeelLocation> tashkeelLocation)
        {
            tashkeelLocation = new List<TashkeelLocation>();
            char[] letters = str.ToCharArray();

            int index = 0;
            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == (char)0x064B)
                { // Tanween Fatha
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064B, i));
                    index++;
                }
                else if (letters[i] == (char)0x064C)
                { // Tanween Damma
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064C, i));
                    index++;
                }
                else if (letters[i] == (char)0x064D)
                { // Tanween Kasra
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064D, i));
                    index++;
                }
                else if (letters[i] == (char)0x064E)
                { // Fatha
                    if (index > 0 && combineTashkeel)
                    {
                        if (tashkeelLocation[index - 1].tashkeel == (char)0x0651) // Shadda
                        {
                            tashkeelLocation[index - 1].tashkeel = (char)0xFC60; // Shadda With Fatha
                            continue;
                        }
                    }

                    tashkeelLocation.Add(new TashkeelLocation((char)0x064E, i));
                    index++;
                }
                else if (letters[i] == (char)0x064F)
                { // DAMMA
                    if (index > 0 && combineTashkeel)
                    {
                        if (tashkeelLocation[index - 1].tashkeel == (char)0x0651)
                        { // SHADDA
                            tashkeelLocation[index - 1].tashkeel = (char)0xFC61; // Shadda With DAMMA
                            continue;
                        }
                    }
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064F, i));
                    index++;
                }
                else if (letters[i] == (char)0x0650)
                { // KASRA
                    if (index > 0 && combineTashkeel)
                    {
                        if (tashkeelLocation[index - 1].tashkeel == (char)0x0651)
                        { // SHADDA
                            tashkeelLocation[index - 1].tashkeel = (char)0xFC62; // Shadda With KASRA
                            continue;
                        }
                    }
                    tashkeelLocation.Add(new TashkeelLocation((char)0x0650, i));
                    index++;
                }
                else if (letters[i] == (char)0x0651)
                { // SHADDA
                    if (index > 0 && combineTashkeel)
                    {
                        if (tashkeelLocation[index - 1].tashkeel == (char)0x064E) // FATHA
                        {
                            tashkeelLocation[index - 1].tashkeel = (char)0xFC60; // Shadda With Fatha
                            continue;
                        }

                        if (tashkeelLocation[index - 1].tashkeel == (char)0x064F) // DAMMA
                        {
                            tashkeelLocation[index - 1].tashkeel = (char)0xFC61; // Shadda With DAMMA
                            continue;
                        }

                        if (tashkeelLocation[index - 1].tashkeel == (char)0x0650) // KASRA
                        {
                            tashkeelLocation[index - 1].tashkeel = (char)0xFC62; // Shadda With KASRA
                            continue;
                        }
                    }

                    tashkeelLocation.Add(new TashkeelLocation((char)0x0651, i));
                    index++;
                }
                else if (letters[i] == (char)0x0652)
                { // SUKUN
                    tashkeelLocation.Add(new TashkeelLocation((char)0x0652, i));
                    index++;
                }
                else if (letters[i] == (char)0x0653)
                { // MADDAH ABOVE
                    tashkeelLocation.Add(new TashkeelLocation((char)0x0653, i));
                    index++;
                }
            }

            string[] split = str.Split(new char[]{(char)0x064B,(char)0x064C,(char)0x064D,
            (char)0x064E,(char)0x064F,(char)0x0650,

            (char)0x0651,(char)0x0652,(char)0x0653,(char)0xFC60,(char)0xFC61,(char)0xFC62});
            str = "";

            foreach (string s in split)
            {
                str += s;
            }

            return str;
        }

        internal static char[] ReturnTashkeel(char[] letters, List<TashkeelLocation> tashkeelLocation)
        {
            char[] lettersWithTashkeel = new char[letters.Length + tashkeelLocation.Count];

            int letterWithTashkeelTracker = 0;
            for (int i = 0; i < letters.Length; i++)
            {
                lettersWithTashkeel[letterWithTashkeelTracker] = letters[i];
                letterWithTashkeelTracker++;
                foreach (TashkeelLocation hLocation in tashkeelLocation)
                {
                    if (hLocation.position == letterWithTashkeelTracker)
                    {
                        lettersWithTashkeel[letterWithTashkeelTracker] = hLocation.tashkeel;
                        letterWithTashkeelTracker++;
                    }
                }
            }

            return lettersWithTashkeel;
        }

        public static bool IsEngOrNumOrSym(char c)
        {
            return char.IsLower(c) || char.IsUpper(c) || char.IsNumber(c) || char.IsSymbol(c);
        }

        public static bool IsClosedToNonReversing(int i, char[] arr)
        {
            return (i > 0 && IsEngOrNumOrSym(arr[i - 1])) || (i < arr.Length - 1 && IsEngOrNumOrSym(arr[i + 1]));
        }

        public static bool IsBetweenNonReversing(int i, char[] arr)
        {
            return i > 0 && i < arr.Length - 1 && IsEngOrNumOrSym(arr[i - 1]) && IsEngOrNumOrSym(arr[i + 1]);
        }

        public static bool IsABracket(char c)
        {
            return c == '(' || c == ')' || c == '[' || c == ']' || c == '>' || c == '<';
        }
        public static char ReverseBrackets(char c)
        {
            if (c == '(')
                return ')';
            else if (c == ')')
                return '(';
            else if (c == '<')
                return '>';
            else if (c == '>')
                return '<';
            else if (c == '[')
                return ']';
            else if (c == ']')
                return '[';
            else return c;
        }

        public static bool IsArabicUnicode(char c)
        {
            return (c >= 0x0600 && c <= 0x06ff) || (c >= 0xfb50 && c <= 0xfdcf) || (c >= 0xfdf0 && c <= 0xfdff);//U+1EE00 to U+1EEFF not included here
        }

        /// <summary>
        /// ����ķ����������������ַ����������ַ���ʽ�滻��������˳��
        /// </summary>
        public static string FixArabicChars(string str)
        {
            List<TashkeelLocation> tashkeelLocation;

            string originString = RemoveTashkeel(str, out tashkeelLocation);

            char[] originalCharArr = originString.ToCharArray();
            char[] fixedCharArr = originString.ToCharArray();

            for (int i = 0; i < originalCharArr.Length; i++)
            {
                originalCharArr[i] = (char)ArabicTable.ArabicMapper.Convert(originalCharArr[i]);
            }

            for (int i = 0; i < originalCharArr.Length; i++)
            {
                bool skip = false;

                // For special Lam Letter connections.
                if (originalCharArr[i] == (char)IsolatedArabicLetters.Lam)
                {

                    if (i < originalCharArr.Length - 1)
                    {
                        //lettersOrigin[i + 1] = (char)ArabicTable.ArabicMapper.Convert(lettersOrigin[i + 1]);
                        if ((originalCharArr[i + 1] == (char)IsolatedArabicLetters.AlefMaksoor))
                        {
                            originalCharArr[i] = (char)0xFEF7;
                            fixedCharArr[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if ((originalCharArr[i + 1] == (char)IsolatedArabicLetters.Alef))
                        {
                            originalCharArr[i] = (char)0xFEF9;
                            fixedCharArr[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if ((originalCharArr[i + 1] == (char)IsolatedArabicLetters.AlefHamza))
                        {
                            originalCharArr[i] = (char)0xFEF5;
                            fixedCharArr[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if ((originalCharArr[i + 1] == (char)IsolatedArabicLetters.AlefMad))
                        {
                            originalCharArr[i] = (char)0xFEF3;
                            fixedCharArr[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                    }

                }

                if (!IsIgnoredCharacter(originalCharArr[i]))
                {
                    if (IsMiddleLetter(originalCharArr, i))
                        fixedCharArr[i] = (char)(originalCharArr[i] + 3);
                    else if (IsFinishingLetter(originalCharArr, i))
                        fixedCharArr[i] = (char)(originalCharArr[i] + 1);
                    else if (IsLeadingLetter(originalCharArr, i))
                        fixedCharArr[i] = (char)(originalCharArr[i] + 2);
                }

                if (skip)
                    i++;

                //chaning numbers to hindu
                if (useHinduNumbers)
                {
                    if (originalCharArr[i] == (char)0x0030)
                        fixedCharArr[i] = (char)0x0660;
                    else if (originalCharArr[i] == (char)0x0031)
                        fixedCharArr[i] = (char)0x0661;
                    else if (originalCharArr[i] == (char)0x0032)
                        fixedCharArr[i] = (char)0x0662;
                    else if (originalCharArr[i] == (char)0x0033)
                        fixedCharArr[i] = (char)0x0663;
                    else if (originalCharArr[i] == (char)0x0034)
                        fixedCharArr[i] = (char)0x0664;
                    else if (originalCharArr[i] == (char)0x0035)
                        fixedCharArr[i] = (char)0x0665;
                    else if (originalCharArr[i] == (char)0x0036)
                        fixedCharArr[i] = (char)0x0666;
                    else if (originalCharArr[i] == (char)0x0037)
                        fixedCharArr[i] = (char)0x0667;
                    else if (originalCharArr[i] == (char)0x0038)
                        fixedCharArr[i] = (char)0x0668;
                    else if (originalCharArr[i] == (char)0x0039)
                        fixedCharArr[i] = (char)0x0669;
                }

            }

            //Return the Tashkeel to their places.
            if (showTashkeel)
                fixedCharArr = ReturnTashkeel(fixedCharArr, tashkeelLocation);

            string fixedStr = new string(fixedCharArr);

            return fixedStr;
        }

        /// <summary>
        /// Converts a string to a form in which the sting will be displayed correctly for arabic text.
        /// </summary>
        /// <param name="str">String to be converted. Example: "Aaa"</param>
        /// <returns>Converted string. Example: "aa aaa A" without the spaces.</returns>
        internal static string FixLine(string str)
        {
            #region PART I Replace Arabic chars
            List<TashkeelLocation> tashkeelLocation;

            string originString = RemoveTashkeel(str, out tashkeelLocation);

            char[] originalCharArr = originString.ToCharArray();
            char[] fixedCharArr = originString.ToCharArray();

            for (int i = 0; i < originalCharArr.Length; i++)
            {
                originalCharArr[i] = (char)ArabicTable.ArabicMapper.Convert(originalCharArr[i]);
            }

            for (int i = 0; i < originalCharArr.Length; i++)
            {
                bool skip = false;

                // For special Lam Letter connections.
                if (originalCharArr[i] == (char)IsolatedArabicLetters.Lam)
                {

                    if (i < originalCharArr.Length - 1)
                    {
                        //lettersOrigin[i + 1] = (char)ArabicTable.ArabicMapper.Convert(lettersOrigin[i + 1]);
                        if ((originalCharArr[i + 1] == (char)IsolatedArabicLetters.AlefMaksoor))
                        {
                            originalCharArr[i] = (char)0xFEF7;
                            fixedCharArr[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if ((originalCharArr[i + 1] == (char)IsolatedArabicLetters.Alef))
                        {
                            originalCharArr[i] = (char)0xFEF9;
                            fixedCharArr[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if ((originalCharArr[i + 1] == (char)IsolatedArabicLetters.AlefHamza))
                        {
                            originalCharArr[i] = (char)0xFEF5;
                            fixedCharArr[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if ((originalCharArr[i + 1] == (char)IsolatedArabicLetters.AlefMad))
                        {
                            originalCharArr[i] = (char)0xFEF3;
                            fixedCharArr[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                    }

                }

                if (!IsIgnoredCharacter(originalCharArr[i]))
                {
                    if (IsMiddleLetter(originalCharArr, i))
                        fixedCharArr[i] = (char)(originalCharArr[i] + 3);
                    else if (IsFinishingLetter(originalCharArr, i))
                        fixedCharArr[i] = (char)(originalCharArr[i] + 1);
                    else if (IsLeadingLetter(originalCharArr, i))
                        fixedCharArr[i] = (char)(originalCharArr[i] + 2);
                }

                if (skip)
                    i++;

                //chaning numbers to hindu
                if (useHinduNumbers)
                {
                    if (originalCharArr[i] == (char)0x0030)
                        fixedCharArr[i] = (char)0x0660;
                    else if (originalCharArr[i] == (char)0x0031)
                        fixedCharArr[i] = (char)0x0661;
                    else if (originalCharArr[i] == (char)0x0032)
                        fixedCharArr[i] = (char)0x0662;
                    else if (originalCharArr[i] == (char)0x0033)
                        fixedCharArr[i] = (char)0x0663;
                    else if (originalCharArr[i] == (char)0x0034)
                        fixedCharArr[i] = (char)0x0664;
                    else if (originalCharArr[i] == (char)0x0035)
                        fixedCharArr[i] = (char)0x0665;
                    else if (originalCharArr[i] == (char)0x0036)
                        fixedCharArr[i] = (char)0x0666;
                    else if (originalCharArr[i] == (char)0x0037)
                        fixedCharArr[i] = (char)0x0667;
                    else if (originalCharArr[i] == (char)0x0038)
                        fixedCharArr[i] = (char)0x0668;
                    else if (originalCharArr[i] == (char)0x0039)
                        fixedCharArr[i] = (char)0x0669;
                }

            }

            //Return the Tashkeel to their places.
            if (showTashkeel)
                fixedCharArr = ReturnTashkeel(fixedCharArr, tashkeelLocation);
            #endregion

            #region PART II Fix string order
            List<char> reversedList = new List<char>();

            List<char> nonReversingList = new List<char>(); //a buffer for non-reversing chars

            for (int i = fixedCharArr.Length - 1; i >= 0; i--)
            {
                //if current char is punctuation && (previous char is punctuation || next char is punctuation), add to reversedList
                if (char.IsPunctuation(fixedCharArr[i]) && i > 0 && i < fixedCharArr.Length - 1 &&
                    (char.IsPunctuation(fixedCharArr[i - 1]) || char.IsPunctuation(fixedCharArr[i + 1])))
                {
                    if (IsABracket(fixedCharArr[i]))
                        reversedList.Add(ReverseBrackets(fixedCharArr[i]));
                    else if (fixedCharArr[i] != 0xFFFF)
                        reversedList.Add(fixedCharArr[i]);
                }
                // For cases where english words and arabic are mixed. This allows for using arabic, english and numbers in one sentence.
                // if current char is space and previous or following char is non-reversing
                else if (fixedCharArr[i] == ' ' && IsBetweenNonReversing(i, fixedCharArr))
                {
                    nonReversingList.Add(fixedCharArr[i]);
                }
                // if current char is non-reversing, add it to nonReversingList
                else if (IsEngOrNumOrSym(fixedCharArr[i]) || (char.IsPunctuation(fixedCharArr[i]) && IsBetweenNonReversing(i, fixedCharArr)))
                {
                    if (IsABracket(fixedCharArr[i]))
                        nonReversingList.Add(ReverseBrackets(fixedCharArr[i]));
                    else
                        nonReversingList.Add(fixedCharArr[i]);
                }
                //illegal Unicode
                else if ((fixedCharArr[i] >= (char)0xD800 && fixedCharArr[i] <= (char)0xDBFF) ||
                        (fixedCharArr[i] >= (char)0xDC00 && fixedCharArr[i] <= (char)0xDFFF))
                {
                    nonReversingList.Add(fixedCharArr[i]);
                }
                else //Arabic chars
                {
                    if (nonReversingList.Count > 0)
                    {
                        for (int j = 0; j < nonReversingList.Count; j++)
                            reversedList.Add(nonReversingList[nonReversingList.Count - 1 - j]);
                        nonReversingList.Clear();
                    }
                    if (fixedCharArr[i] != 0xFFFF)
                        reversedList.Add(fixedCharArr[i]);

                }
            }
            if (nonReversingList.Count > 0)
            {
                for (int j = 0; j < nonReversingList.Count; j++)
                    reversedList.Add(nonReversingList[nonReversingList.Count - 1 - j]);
                nonReversingList.Clear();
            }

            // Moving letters from a list to an array.
            fixedCharArr = new char[reversedList.Count];
            for (int i = 0; i < fixedCharArr.Length; i++)
                fixedCharArr[i] = reversedList[i];


            str = new string(fixedCharArr);
            return str;

            #endregion
        }

        /// <summary>
        /// English letters, numbers and punctuation characters are ignored. This checks if the ch is an ignored character.
        /// </summary>
        /// <param name="ch">The character to be checked for skipping</param>
        /// <returns>True if the character should be ignored, false if it should not be ignored.</returns>
        internal static bool IsIgnoredCharacter(char ch)
        {
            bool isPunctuation = char.IsPunctuation(ch);
            bool isNumber = char.IsNumber(ch);
            bool isLower = char.IsLower(ch);
            bool isUpper = char.IsUpper(ch);
            bool isSymbol = char.IsSymbol(ch);
            bool isPersianCharacter = ch == (char)0xFB56 || ch == (char)0xFB7A || ch == (char)0xFB8A || ch == (char)0xFB92 || ch == (char)0xFB8E;
            bool isPresentationFormB = (ch <= (char)0xFEFF && ch >= (char)0xFE70);
            bool isAcceptableCharacter = isPresentationFormB || isPersianCharacter || ch == (char)0xFBFC;



            return isPunctuation ||
                isNumber ||
                    isLower ||
                    isUpper ||
                    isSymbol ||
                    !isAcceptableCharacter ||
                    ch == 'a' || ch == '>' || ch == '<' || ch == (char)0x061B;

            //            return char.IsPunctuation(ch) || char.IsNumber(ch) || ch == 'a' || ch == '>' || ch == '<' ||
            //                    char.IsLower(ch) || char.IsUpper(ch) || ch == (char)0x061B || char.IsSymbol(ch)
            //					|| !(ch <= (char)0xFEFF && ch >= (char)0xFE70) // Presentation Form B
            //					|| ch == (char)0xFB56 || ch == (char)0xFB7A || ch == (char)0xFB8A || ch == (char)0xFB92; // Persian Characters

            //					PersianPe = 0xFB56,
            //		PersianChe = 0xFB7A,
            //		PersianZe = 0xFB8A,
            //		PersianGaf = 0xFB92
            //lettersOrigin[i] <= (char)0xFEFF && lettersOrigin[i] >= (char)0xFE70
        }

        /// <summary>
        /// Checks if the letter at index value is a leading character in Arabic or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a leading character, else, returns false</returns>
        internal static bool IsLeadingLetter(char[] letters, int index)
        {

            bool lettersThatCannotBeBeforeALeadingLetter = index == 0
                || letters[index - 1] == ' '
                    || letters[index - 1] == '*' // ??? Remove?
                    || letters[index - 1] == 'A' // ??? Remove?
                    || char.IsPunctuation(letters[index - 1])
                    || letters[index - 1] == '>'
                    || letters[index - 1] == '<'
                    || letters[index - 1] == (int)IsolatedArabicLetters.Alef
                    || letters[index - 1] == (int)IsolatedArabicLetters.Dal
                    || letters[index - 1] == (int)IsolatedArabicLetters.Thal
                    || letters[index - 1] == (int)IsolatedArabicLetters.Ra2
                    || letters[index - 1] == (int)IsolatedArabicLetters.Zeen
                    || letters[index - 1] == (int)IsolatedArabicLetters.PersianZe
                    //|| letters[index - 1] == (int)IsolatedArabicLetters.AlefMaksora 
                    || letters[index - 1] == (int)IsolatedArabicLetters.Waw
                    || letters[index - 1] == (int)IsolatedArabicLetters.AlefMad
                    || letters[index - 1] == (int)IsolatedArabicLetters.AlefHamza
                    || letters[index - 1] == (int)IsolatedArabicLetters.Hamza
                    || letters[index - 1] == (int)IsolatedArabicLetters.AlefMaksoor
                    || letters[index - 1] == (int)IsolatedArabicLetters.WawHamza;

            bool lettersThatCannotBeALeadingLetter = letters[index] != ' '
                && letters[index] != (int)IsolatedArabicLetters.Dal
                && letters[index] != (int)IsolatedArabicLetters.Thal
                    && letters[index] != (int)IsolatedArabicLetters.Ra2
                    && letters[index] != (int)IsolatedArabicLetters.Zeen
                    && letters[index] != (int)IsolatedArabicLetters.PersianZe
                    && letters[index] != (int)IsolatedArabicLetters.Alef
                    && letters[index] != (int)IsolatedArabicLetters.AlefHamza
                    && letters[index] != (int)IsolatedArabicLetters.AlefMaksoor
                    && letters[index] != (int)IsolatedArabicLetters.AlefMad
                    && letters[index] != (int)IsolatedArabicLetters.WawHamza
                    && letters[index] != (int)IsolatedArabicLetters.Waw
                    && letters[index] != (int)IsolatedArabicLetters.Hamza;

            bool lettersThatCannotBeAfterLeadingLetter = index < letters.Length - 1
                && letters[index + 1] != ' '
                    && !char.IsPunctuation(letters[index + 1])
                    && !char.IsNumber(letters[index + 1])
                    && !char.IsSymbol(letters[index + 1])
                    && !char.IsLower(letters[index + 1])
                    && !char.IsUpper(letters[index + 1])
                    && letters[index + 1] != (int)IsolatedArabicLetters.Hamza;

            if (lettersThatCannotBeBeforeALeadingLetter && lettersThatCannotBeALeadingLetter && lettersThatCannotBeAfterLeadingLetter)

            //		if ((index == 0 || letters[index - 1] == ' ' || letters[index - 1] == '*' || letters[index - 1] == 'A' || char.IsPunctuation(letters[index - 1])
            //		     || letters[index - 1] == '>' || letters[index - 1] == '<' 
            //		     || letters[index - 1] == (int)IsolatedArabicLetters.Alef
            //		     || letters[index - 1] == (int)IsolatedArabicLetters.Dal || letters[index - 1] == (int)IsolatedArabicLetters.Thal
            //		     || letters[index - 1] == (int)IsolatedArabicLetters.Ra2 
            //		     || letters[index - 1] == (int)IsolatedArabicLetters.Zeen || letters[index - 1] == (int)IsolatedArabicLetters.PersianZe
            //		     || letters[index - 1] == (int)IsolatedArabicLetters.AlefMaksora || letters[index - 1] == (int)IsolatedArabicLetters.Waw
            //		     || letters[index - 1] == (int)IsolatedArabicLetters.AlefMad || letters[index - 1] == (int)IsolatedArabicLetters.AlefHamza
            //		     || letters[index - 1] == (int)IsolatedArabicLetters.AlefMaksoor || letters[index - 1] == (int)IsolatedArabicLetters.WawHamza) 
            //		    && letters[index] != ' ' && letters[index] != (int)IsolatedArabicLetters.Dal
            //		    && letters[index] != (int)IsolatedArabicLetters.Thal
            //		    && letters[index] != (int)IsolatedArabicLetters.Ra2 
            //		    && letters[index] != (int)IsolatedArabicLetters.Zeen && letters[index] != (int)IsolatedArabicLetters.PersianZe
            //		    && letters[index] != (int)IsolatedArabicLetters.Alef && letters[index] != (int)IsolatedArabicLetters.AlefHamza
            //		    && letters[index] != (int)IsolatedArabicLetters.AlefMaksoor
            //		    && letters[index] != (int)IsolatedArabicLetters.AlefMad
            //		    && letters[index] != (int)IsolatedArabicLetters.WawHamza
            //		    && letters[index] != (int)IsolatedArabicLetters.Waw
            //		    && letters[index] != (int)IsolatedArabicLetters.Hamza
            //		    && index < letters.Length - 1 && letters[index + 1] != ' ' && !char.IsPunctuation(letters[index + 1] ) && !char.IsNumber(letters[index + 1])
            //		    && letters[index + 1] != (int)IsolatedArabicLetters.Hamza )
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Checks if the letter at index value is a finishing character in Arabic or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a finishing character, else, returns false</returns>
        internal static bool IsFinishingLetter(char[] letters, int index)
        {
            bool indexZero = index != 0;
            bool lettersThatCannotBeBeforeAFinishingLetter = (index == 0) ? false :
                    letters[index - 1] != ' '
                    //				&& char.IsDigit(letters[index-1])
                    //				&& char.IsLower(letters[index-1])
                    //				&& char.IsUpper(letters[index-1])
                    //				&& char.IsNumber(letters[index-1])
                    //				&& char.IsWhiteSpace(letters[index-1])
                    //				&& char.IsPunctuation(letters[index-1])
                    //				&& char.IsSymbol(letters[index-1])

                    && letters[index - 1] != (int)IsolatedArabicLetters.Dal
                    && letters[index - 1] != (int)IsolatedArabicLetters.Thal
                    && letters[index - 1] != (int)IsolatedArabicLetters.Ra2
                    && letters[index - 1] != (int)IsolatedArabicLetters.Zeen
                    && letters[index - 1] != (int)IsolatedArabicLetters.PersianZe
                    //&& letters[index - 1] != (int)IsolatedArabicLetters.AlefMaksora 
                    && letters[index - 1] != (int)IsolatedArabicLetters.Waw
                    && letters[index - 1] != (int)IsolatedArabicLetters.Alef
                    && letters[index - 1] != (int)IsolatedArabicLetters.AlefMad
                    && letters[index - 1] != (int)IsolatedArabicLetters.AlefHamza
                    && letters[index - 1] != (int)IsolatedArabicLetters.AlefMaksoor
                    && letters[index - 1] != (int)IsolatedArabicLetters.WawHamza
                    && letters[index - 1] != (int)IsolatedArabicLetters.Hamza



                    && !char.IsPunctuation(letters[index - 1])
                    && !char.IsSymbol(letters[index - 1])
                    && letters[index - 1] != '>'
                    && letters[index - 1] != '<';


            bool lettersThatCannotBeFinishingLetters = letters[index] != ' ' && letters[index] != (int)IsolatedArabicLetters.Hamza;




            if (lettersThatCannotBeBeforeAFinishingLetter && lettersThatCannotBeFinishingLetters)

            //		if (index != 0 && letters[index - 1] != ' ' && letters[index - 1] != '*' && letters[index - 1] != 'A'
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.Dal && letters[index - 1] != (int)IsolatedArabicLetters.Thal
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.Ra2 
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.Zeen && letters[index - 1] != (int)IsolatedArabicLetters.PersianZe
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.AlefMaksora && letters[index - 1] != (int)IsolatedArabicLetters.Waw
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.Alef && letters[index - 1] != (int)IsolatedArabicLetters.AlefMad
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.AlefHamza && letters[index - 1] != (int)IsolatedArabicLetters.AlefMaksoor
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.WawHamza && letters[index - 1] != (int)IsolatedArabicLetters.Hamza 
            //		    && !char.IsPunctuation(letters[index - 1]) && letters[index - 1] != '>' && letters[index - 1] != '<' 
            //		    && letters[index] != ' ' && index < letters.Length
            //		    && letters[index] != (int)IsolatedArabicLetters.Hamza)
            {
                //try
                //{
                //    if (char.IsPunctuation(letters[index + 1]))
                //        return true;
                //    else
                //        return false;
                //}
                //catch (Exception e)
                //{
                //    return false;
                //}

                return true;
            }
            //return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the letter at index value is a middle character in Arabic or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a middle character, else, returns false</returns>
        internal static bool IsMiddleLetter(char[] letters, int index)
        {
            bool lettersThatCannotBeMiddleLetters = (index == 0) ? false :
                letters[index] != (int)IsolatedArabicLetters.Alef
                    && letters[index] != (int)IsolatedArabicLetters.Dal
                    && letters[index] != (int)IsolatedArabicLetters.Thal
                    && letters[index] != (int)IsolatedArabicLetters.Ra2
                    && letters[index] != (int)IsolatedArabicLetters.Zeen
                    && letters[index] != (int)IsolatedArabicLetters.PersianZe
                    //&& letters[index] != (int)IsolatedArabicLetters.AlefMaksora
                    && letters[index] != (int)IsolatedArabicLetters.Waw
                    && letters[index] != (int)IsolatedArabicLetters.AlefMad
                    && letters[index] != (int)IsolatedArabicLetters.AlefHamza
                    && letters[index] != (int)IsolatedArabicLetters.AlefMaksoor
                    && letters[index] != (int)IsolatedArabicLetters.WawHamza
                    && letters[index] != (int)IsolatedArabicLetters.Hamza;

            bool lettersThatCannotBeBeforeMiddleCharacters = (index == 0) ? false :
                    letters[index - 1] != (int)IsolatedArabicLetters.Alef
                    && letters[index - 1] != (int)IsolatedArabicLetters.Dal
                    && letters[index - 1] != (int)IsolatedArabicLetters.Thal
                    && letters[index - 1] != (int)IsolatedArabicLetters.Ra2
                    && letters[index - 1] != (int)IsolatedArabicLetters.Zeen
                    && letters[index - 1] != (int)IsolatedArabicLetters.PersianZe
                    //&& letters[index - 1] != (int)IsolatedArabicLetters.AlefMaksora
                    && letters[index - 1] != (int)IsolatedArabicLetters.Waw
                    && letters[index - 1] != (int)IsolatedArabicLetters.AlefMad
                    && letters[index - 1] != (int)IsolatedArabicLetters.AlefHamza
                    && letters[index - 1] != (int)IsolatedArabicLetters.AlefMaksoor
                    && letters[index - 1] != (int)IsolatedArabicLetters.WawHamza
                    && letters[index - 1] != (int)IsolatedArabicLetters.Hamza
                    && !char.IsPunctuation(letters[index - 1])
                    && letters[index - 1] != '>'
                    && letters[index - 1] != '<'
                    && letters[index - 1] != ' '
                    && letters[index - 1] != '*';

            bool lettersThatCannotBeAfterMiddleCharacters = (index >= letters.Length - 1) ? false :
                letters[index + 1] != ' '
                    && letters[index + 1] != '\r'
                    && letters[index + 1] != (int)IsolatedArabicLetters.Hamza
                    && !char.IsNumber(letters[index + 1])
                    && !char.IsSymbol(letters[index + 1])
                    && !char.IsPunctuation(letters[index + 1]);
            if (lettersThatCannotBeAfterMiddleCharacters && lettersThatCannotBeBeforeMiddleCharacters && lettersThatCannotBeMiddleLetters)

            //		if (index != 0 && letters[index] != ' '
            //		    && letters[index] != (int)IsolatedArabicLetters.Alef && letters[index] != (int)IsolatedArabicLetters.Dal
            //		    && letters[index] != (int)IsolatedArabicLetters.Thal && letters[index] != (int)IsolatedArabicLetters.Ra2
            //		    && letters[index] != (int)IsolatedArabicLetters.Zeen && letters[index] != (int)IsolatedArabicLetters.PersianZe 
            //		    && letters[index] != (int)IsolatedArabicLetters.AlefMaksora
            //		    && letters[index] != (int)IsolatedArabicLetters.Waw && letters[index] != (int)IsolatedArabicLetters.AlefMad
            //		    && letters[index] != (int)IsolatedArabicLetters.AlefHamza && letters[index] != (int)IsolatedArabicLetters.AlefMaksoor
            //		    && letters[index] != (int)IsolatedArabicLetters.WawHamza && letters[index] != (int)IsolatedArabicLetters.Hamza
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.Alef && letters[index - 1] != (int)IsolatedArabicLetters.Dal
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.Thal && letters[index - 1] != (int)IsolatedArabicLetters.Ra2
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.Zeen && letters[index - 1] != (int)IsolatedArabicLetters.PersianZe 
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.AlefMaksora
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.Waw && letters[index - 1] != (int)IsolatedArabicLetters.AlefMad
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.AlefHamza && letters[index - 1] != (int)IsolatedArabicLetters.AlefMaksoor
            //		    && letters[index - 1] != (int)IsolatedArabicLetters.WawHamza && letters[index - 1] != (int)IsolatedArabicLetters.Hamza 
            //		    && letters[index - 1] != '>' && letters[index - 1] != '<' 
            //		    && letters[index - 1] != ' ' && letters[index - 1] != '*' && !char.IsPunctuation(letters[index - 1])
            //		    && index < letters.Length - 1 && letters[index + 1] != ' ' && letters[index + 1] != '\r' && letters[index + 1] != 'A' 
            //		    && letters[index + 1] != '>' && letters[index + 1] != '>' && letters[index + 1] != (int)IsolatedArabicLetters.Hamza
            //		    )
            {
                try
                {
                    if (char.IsPunctuation(letters[index + 1]))
                        return false;
                    else
                        return true;
                }
                catch
                {
                    return false;
                }
                //return true;
            }
            else
                return false;
        }


    }
}