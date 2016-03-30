using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HTE.GBA
{
    /*public class TextManager
    {
        private Dictionary<string, string[]> texts;

        public TextManager()
        {
            texts = new Dictionary<string, string[]>();
        }

        public void Register
    }*/
    
    public class TextTable
    {
        private static string[] eng = new string[] { " ", "À", "Á", "Â", "Ç", "È", "É", "Ê", "Ë", "Ì", "こ", "Î", "Ï", "Ò", "Ó", "Ô", "Œ", "Ù", "Ú", "Û", "Ñ", "ß", "à", "á", "ね", "ç", "è", "é", "ê", "ë", "ì", "ま", "î", "ï", "ò", "ó", "ô", "œ", "ù", "ú", "û", "ñ", "º", "ª", "[o]", "&", "+", "あ", "ぃ", "ぅ", "ぇ", "ぉ", "[Lv]", "=", "ょ", "が", "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ", "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び", "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ", "っ", "¿", "¡", "[pk]", "[mn]", "[po]", "[ké]", "[bl]", "[oc]", "[k]", "Í", "%", "(", ")", "セ", "ソ", "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "â", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "í", "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "[U]", "[D]", "[L]", "[R]", "ヲ", "ン", "ァ", "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ", "ガ", "ギ", "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ", "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ", "ッ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "!", "?", ".", "-", "·", "[...]", "“", "”", "‘", "'", "♂", "♀", "$", ",", "*", "/", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "►", ":", "Ä", "Ö", "Ü", "ä", "ö", "ü", "↑", "↓", "←", "\\l", "\\p", "\\c", "\\v", "\\n", "\\x" };
        //                                                                                                                                                                                      1B                                                                                                                                          35                                                                                                                                                                             53                                                                                                                                                                                                                                                                                                                                                                                                       94                                                               9F    A0   A1                                           AA                                   B1  B2   B3   B4    B5   B6   B7                  BB                                                                                                                           D4   D5                                                                                                                           EE   EF   F0   F1   F2   F3   F4   F5   F6   F7   F8   F9   FA    FB      FC      FD      FE     FF
        private static string[] jap = new string[] { " ", "あ", "い", "う", "え", "お", "か", "き", "く", "け", "こ", "さ", "し", "す", "せ", "そ", "た", "ち", "つ", "て", "と", "な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ", "ま", "み", "む", "め", "も", "や", "ゆ", "よ", "ら", "り", "る", "れ", "ろ", "わ", "を", "ん", "ぁ", "ぃ", "ぅ", "ぇ", "ぉ", "ゃ", "ゅ", "ょ", "が", "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ", "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び", "ぶ", "べ", "ぼ", "ぱ", "ぴ", "ぷ", "ぺ", "ぽ", "っ", "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ", "サ", "シ", ")", "セ", "ス", "タ", "チ", "ツ", "テ", "ト", "ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ", "マ", "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ", "ル", "レ", "ロ", "ワ", "ヲ", "ン", "ァ", "ィ", "ゥ", "ェ", "ォ", "ャ", "ュ", "ョ", "ガ", "ギ", "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ", "ベ", "ボ", "パ", "ピ", "プ", "ペ", "ポ", "ッ", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "！", "？", "。", "ー", "・", "[・・]", "『", "』", "「", "」", "♂", "♀", "円", ".", "×", "/", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "►", ":", "Ä", "Ö", "Ü", "ä", "ö", "ü", "↑", "↓", "←", "\\l", "\\p", "\\c", "\\v", "\\n", "\\x" };
        //                                                                                                                                                                                      1B                                                                                                                                          35                                                                                                                                                                             53                                                                                                                                                                                                                                                                                                                                                                                                       94                                                               9F    A0   A1                                           AA                                     B1   B2     B3   B4    B5   B6   B7                      BB                                                                                                                           D4   D5                                                                                                                           EE   EF   F0   F1   F2   F3   F4   F5   F6   F7   F8   F9   FA    FB      FC      FD      FE     FF

        private static HashSet<char> validEscapes = new HashSet<char>(new char[] { 'h', 'l', 'p', 'c', 'v', 'n', 'x' });

        public enum GBACharSet
        {
            English, Japanese
        }

        public static string GetString(byte[] bytes, bool shearTerminator = true, GBACharSet charSet = GBACharSet.English)
        {
            /*string s = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0xFF) break;
                s += eng[bytes[i]];
            }
            return s;*/

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                sb.Append(charSet == GBACharSet.English ? eng[b] : jap[b]);

                if (b >= 0xFC)
                {
                    if (b == 0xFF) break;
                    else if (b == 0xFC || b == 0xFD) // functions
                    {
                        i++;
                        sb.Append("\\h" + bytes[i].ToString("X2"));
                    }
                }
            }

            if (shearTerminator)
            {
                sb = sb.Replace("\\x", "");
            }

            return sb.ToString();
        }

        public static byte[] GetBytes(string s, GBACharSet charSet = GBACharSet.English)
        {
            #region Old
            /*List<string> trans = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '[')
                {
                    string ss = "";// = c.ToString();
                    while (i < s.Length && c != ']')
                    {
                        ss += c.ToString();

                        i++;
                        c = s[i];
                    }
                    trans.Add(ss + c);
                }
                else if (c == '\\')
                {
                    i++;
                    trans.Add("\\" + s[i]);
                }
                else
                {
                    trans.Add(c.ToString());
                }
            }

            byte[] buffer = new byte[trans.Count];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = GetByte(trans[i]);
            }

            return buffer;*/
            #endregion

            try
            {
                // First, split the string into pieces
                string[] exploded = ExplodeString(s);

                // The get the bytes for each part
                List<byte> result = new List<byte>();
                foreach (string str in exploded)
                {
                    result.Add(GetByte(str, charSet));
                }
                return result.ToArray();
            }
            catch (Exception ex)
            {
                return new byte[0];
            }
        }

        public static byte[] GetBytes(string s, int length, GBACharSet charSet = GBACharSet.English)
        {
            byte[] b = GetBytes(s);

            // TODO: improve this~~

            if (b.Length != length)
            {
                List<byte> buffer = b.ToList();
                if (b.Length > length)
                {
                    Array.Resize(ref b, length);
                    return b;
                }
                else if (b.Length < length)
                {
                    buffer.Add(0xFF);
                    for (int i = 1; i < length - buffer.Count - 1; i++)
                    {
                        buffer.Add(00);
                    }
                    return buffer.ToArray();
                }

                return buffer.ToArray();
            }
            else
            {
                return b;
            }
        }

        public static byte GetByte(string s, GBACharSet charSet = GBACharSet.English)
        {
            // \\h escape
            if (s.StartsWith("\\h"))
            {
                return byte.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber);
            }

            // otherwise
            for (int i = 0; i < eng.Length; i++)
            {
                if ((charSet == GBACharSet.English ? eng[i] : jap[i]) == s)
                {
                    return (byte)i;
                }

                //if (eng[i] == s)
                //{
                //    return (byte)i;
                //}
            }
            return 0;
        }

        private static string[] ExplodeString(string s)
        {
            List<string> exploded = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (c == '[')
                {
                    /*int end = s.IndexOf(']', i);
                    if (end == -1) throw new Exception("Unterminated '[]' character!");*/

                    // Find and build the []
                    StringBuilder sb = new StringBuilder();
                    int end = -1;
                    for (int j = i; j < s.Length; j++)
                    {
                        sb.Append(s[j]);

                        if (s[j] == ']')
                        {
                            end = j;
                            break;
                        }
                    }

                    if (end == -1) throw new Exception("Unterminated '[]' character!");

                    //exploded.Add(s.Substring(i, end - i + 1));

                    exploded.Add(sb.ToString());
                    i = end;
                }
                else if (c == '\\')
                {
                    try
                    {
                        char e = s[i + 1];
                        if (!validEscapes.Contains(e)) throw new Exception();

                        if (e == 'h')
                        {
                            // copy next two characters
                            string bbbbb = s.Substring(i + 2, 2);
                            i += 2;

                            // a raw test
                            // will throw an exception if invalid
                            int.Parse(bbbbb, System.Globalization.NumberStyles.HexNumber);

                            exploded.Add("\\" + e + bbbbb);
                        }
                        else
                        {
                            exploded.Add("\\" + e);
                        }

                        i++;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Invalid escape sequence!");
                    }
                }
                else
                {
                    exploded.Add(c.ToString());
                }
            }

            return exploded.ToArray();
        }

        public static int GetStringLength(string s)
        {
            try
            {
                return ExplodeString(s).Length;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
