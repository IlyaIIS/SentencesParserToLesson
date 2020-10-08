using System;
using System.Collections.Generic;
using System.Linq;

namespace SentencesParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> nGrams = new Dictionary<string, string>();
            List<List<string>> sentence = new List<List<string>>();

            Console.WriteLine("Введите максимальный размер N-грамм.");
            Console.Write("N=");
            int nMax = GetInt("Введите число!");

            Console.WriteLine("Введите текст:");
            sentence = SplitSentence(Console.ReadLine());       //"a b.a c.b a.b b.b b. d e f. d e d. d e f g h w z. h w a"

            //nGrams = ParceTwoAndThreeGrams(biGram, triGram);         //Анализ би и триграмм
            nGrams = ParceNGrams(FindNGrams(sentence, 2));
            for (int i = 3; i <= nMax; i++)
                nGrams = nGrams.Concat(ParceNGrams(FindNGrams(sentence, i))).ToDictionary(x => x.Key, x => x.Value);

            for (int i = 0; i < nGrams.Keys.Count; i++)              //Вывод результата
            {
                string key = nGrams.ElementAt(i).Key;
                Console.WriteLine(key + ": " + nGrams[key]);
            }

            
        }
        static int GetInt(string textToUser)              //Требует у пользователя число
        {
            bool repeat;
            int output;
            do
            {
                repeat = !Int32.TryParse(Console.ReadLine(), out output);
                if (repeat) Console.WriteLine(textToUser);
            } while (repeat);

            return output;
        }
        static Dictionary<string, string> ParceTwoAndThreeGrams(List<string[]> biGram, List<string[]> triGram)
        {
            Dictionary<string, string> nGrams = new Dictionary<string, string>();

            for (int num = 0; num < biGram.Count;)
            {
                Dictionary<string, int> valueDict = new Dictionary<string, int>();
                string key = biGram[num][0];
                for (int i = num; i < biGram.Count; i++)               //формирование словаря с количеством сочитаний key - value
                {
                    if (biGram[i][0] == key)
                    {
                        if (valueDict.ContainsKey(biGram[i][1]))
                            valueDict[biGram[i][1]]++;
                        else
                            valueDict.Add(biGram[i][1], 1);

                        biGram.RemoveAt(i);
                        i--;
                    }
                }

                int max = valueDict.Values.Max();

                for (int i = 0; i < valueDict.Keys.Count; i++)              //Убирает из словаря редко встреченные элементы
                {
                    string k = valueDict.ElementAt(i).Key;
                    if (valueDict[k] < max) { valueDict.Remove(k); i--; }
                }

                for (int i = 0; i < valueDict.Keys.Count - 1;)              //Убирает из словаря элементы по лексикографическому признаку
                {
                    string kNow = valueDict.ElementAt(i).Key;
                    string kNext = valueDict.ElementAt(i + 1).Key;
                    if (String.CompareOrdinal(kNow, 0, kNext, 0, Math.Max(kNow.Length, kNext.Length)) < 0)   // 1 если левый хуже, -1 если правый хуже
                        valueDict.Remove(kNext);
                    else
                        valueDict.Remove(kNow);
                }

                nGrams.Add(key, valueDict.ElementAt(0).Key);
            }

            for (int num = 0; num < triGram.Count;)
            {
                Dictionary<string, int> valueDict = new Dictionary<string, int>();
                string[] key = { triGram[num][0], triGram[num][1] };                           //
                for (int i = num; i < triGram.Count; i++)               //формирование словаря с количеством сочитаний key - value
                {
                    if (triGram[i][0] == key[0] && triGram[i][1] == key[1])                           //
                    {
                        if (valueDict.ContainsKey(triGram[i][2]))
                            valueDict[triGram[i][2]]++;
                        else
                            valueDict.Add(triGram[i][2], 1);

                        triGram.RemoveAt(i);
                        i--;
                    }
                }

                int max = valueDict.Values.Max();

                for (int i = 0; i < valueDict.Keys.Count; i++)              //Убирает из словаря редко встреченные элементы
                {
                    string k = valueDict.ElementAt(i).Key;
                    if (valueDict[k] < max) { valueDict.Remove(k); i--; }
                }

                for (int i = 0; i < valueDict.Keys.Count - 1;)              //Убирает из словаря элементы по лексикографическому признаку
                {
                    string kNow = valueDict.ElementAt(i).Key;
                    string kNext = valueDict.ElementAt(i + 1).Key;
                    if (String.CompareOrdinal(kNow, 0, kNext, 0, Math.Max(kNow.Length, kNext.Length)) < 0)   // 1 если левый хуже, -1 если правый хуже
                        valueDict.Remove(kNext);
                    else
                        valueDict.Remove(kNow);
                }

                nGrams.Add(key[0] + " " + key[1], valueDict.ElementAt(0).Key);
            }

            return nGrams;
        }

        static Dictionary<string, string> ParceNGrams(List<string[]> nGram)
        {
            Dictionary<string, string> nGrams = new Dictionary<string, string>();

            for (int num = 0; num < nGram.Count;)
            {
                Dictionary<string, int> valueDict = new Dictionary<string, int>();
                int N = nGram[0].Length;
                string[] key = new string[N-1];

                for (int i = 0; i < N-1; i++) key[i] = nGram[num][i];   //формирование массива начала N-граммы

                for (int i = num; i < nGram.Count; i++)                 //формирование словаря концов N-грамма с кол-вом их сочетаний с key
                {
                    if (IsBeginningNGram(nGram, key, N, i))
                    {
                        if (valueDict.ContainsKey(nGram[i][N - 1]))
                            valueDict[nGram[i][N - 1]]++;
                        else
                            valueDict.Add(nGram[i][N - 1], 1);

                        nGram.RemoveAt(i);
                        i--;
                    }
                }

                int max = valueDict.Values.Max();

                for (int i = 0; i < valueDict.Keys.Count; i++)              //Убирает редко встреченные окончания N-грамма
                {
                    string k = valueDict.ElementAt(i).Key;
                    if (valueDict[k] < max) { valueDict.Remove(k); i--; }
                }

                for (int i = 0; i < valueDict.Keys.Count - 1;)              //Отберает окончание по лексикографическому признаку
                {
                    string kNow = valueDict.ElementAt(i).Key;
                    string kNext = valueDict.ElementAt(i + 1).Key;
                    if (String.CompareOrdinal(kNow, 0, kNext, 0, Math.Max(kNow.Length, kNext.Length)) < 0)   // 1 если левый хуже, -1 если правый хуже
                        valueDict.Remove(kNext);
                    else
                        valueDict.Remove(kNow);
                }

                string keyStr = key[0];
                for (int i = 1; i < N - 1; i++) keyStr += " " + key[i];

                nGrams.Add(keyStr, valueDict.ElementAt(0).Key);
            }

            return nGrams;

            static bool IsBeginningNGram(List<string[]> nGram, string[] key, int N, int posNow)  //проверка, является ли key началом N-граммы
            {
                bool output = true;
                for (int i = 0; i < N-1; i++)
                {
                    output = output && nGram[posNow][i] == key[i];
                }

                return output;
            }
        }

            static List<string[]> FindNGrams(List<List<string>> sentence, int N)      //Возвращает лист Nмерных масивов Nграмм
        {
            List<string[]> NGram = new List<string[]>();

            for (int senNum = 0; senNum < sentence.Count; senNum++)
            {
                if (sentence[senNum].Count >= N)
                {
                    for (int wordNum = 0; wordNum < sentence[senNum].Count - (N-1); wordNum++)
                    {
                        NGram.Add(new string[N] );
                        for (int i = 0; i < N; i++)
                        {
                            NGram[NGram.Count-1][i] = sentence[senNum][wordNum + i];
                        }
                    }
                }
            }

            return NGram;
        }
        static List<List<string>> SplitSentence(String str)
        {
            str = " " + str;                                          //на случай пустых строк
            char[] sign = { '.', '?', '!', ';', '(', ')' };           //список символов, отделяющих предложения
            List<List<string>> sentence = new List<List<string>>();

            int sentNum = 0, wordNum = 0, i = 0;
            sentence.Add(new List<string>());                         //деление текста на предложения со словами
            do
            {
                if (Char.IsLetter(str[i]) || str[i] == '\'')
                {
                    sentence[sentNum].Add("");
                    do
                    {
                        sentence[sentNum][wordNum] += str[i];
                        i++;
                    } while ((i < str.Length) && (Char.IsLetter(str[i]) || str[i] == '\''));
                    wordNum++;
                    i--;
                }
                else
                if (sign.Contains(str[i]))
                {
                    wordNum = 0;
                    sentence.Add(new List<string>());
                    sentNum++;
                }

                i++;
            } while (i < str.Length);

            for (int sen = 0; sen < sentence.Count; sen++)             //удаление пустых предложений
            {
                if (sentence[sen].Count == 0) sentence.RemoveAt(sen);
            }

            /*
            Console.WriteLine(sentence.Count);
            for (int sen = 0; sen < sentence.Count; sen++)             //вывод данных на случай отладки
            {
                for (int word = 0; word < sentence[sen].Count; word++)
                {
                    Console.Write(sentence[sen][word]);
                    Console.Write(' ');
                }
                Console.WriteLine(' ');
            }
            */

            return sentence;
        }


    }
}
