using System;
using System.Linq;

namespace Szyfry_blokowe
{
    class Program
    {

        public enum Option
        {
            None,
            Encrypt,
            Decrypt
        }

        static void Main(string[] args)
        {
            Menu();
        }


        public static void Menu()
        {
            uint message = 0, mainKey = 0;

            while (true)
            {
                Console.Write("\n Podaj WIADOMOŚĆ jako wartość liczbową dziesiętną pomiędzy 0 a 255:\t");
                try
                {
                    message = uint.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.Clear();
                    continue;
                }
                break;
            }
            Console.Clear();
            while (true)
            {
                try
                {
                    Console.WriteLine($"\n Podaj WIADOMOŚĆ jako wartość liczbową dziesiętną pomiędzy 0 a 255:\t{message}");
                    Console.Write(" Podaj KLUCZ jako wartość liczbową dziesiętną pomiędzy 0 a 255:\t\t");
                    mainKey = uint.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.Clear();
                    continue;
                }
                break;
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(" 1.Szyfruj\n 2.Deszyfruj");
            Console.Write(" Wybieram opcje: ");
            uint option = 0;
            while (true)
            {
                try
                {
                    option = uint.Parse(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine();
                    Console.Write(" Wybieram opcje: ");
                    continue;
                }
                if (option == (uint)Option.Encrypt || option == (uint)Option.Decrypt)
                {
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.Write(" Wybieram opcje: ");
                    continue;
                }

            }
            Console.WriteLine(" -----------------------------------------------------------------------------");
            Console.WriteLine();

            var encryptedMessage = Array.Empty<bool>() ;

            if (option == (uint)Option.Encrypt)
            {
                encryptedMessage = FeistelsPermutation(message, mainKey, 1);

                Console.Write($" WIADOMOŚĆ po szyfrowaniu to:\t ");
                foreach (var item in encryptedMessage)
                {
                    byte n = item == true ? (byte)1 : (byte)0;
                    Console.Write(n);
                }
                uint encryptedMessageDecimal = ConvertBinaryToDecimal(encryptedMessage);

                Console.WriteLine($" czyli {encryptedMessageDecimal}");
            }
            else
            {
                encryptedMessage = FeistelsPermutation(message, mainKey, 2);

                Console.Write($" WIADOMOŚĆ po odszyfrowaniu to:\t ");
                foreach (var item in encryptedMessage)
                {
                    byte n = item == true ? (byte)1 : (byte)0;
                    Console.Write(n);
                }

                uint encryptedMessageDecimal = ConvertBinaryToDecimal(encryptedMessage);

                Console.WriteLine($" czyli {encryptedMessageDecimal}");
            }
        }

        public static bool[] ConvertDecimalToBinary(uint number)
        {
            bool[] binaryNumber = new bool[8];
            int counter = 1;
            while (number >= 1)
            {
                if (number % 2 == 0) binaryNumber[^counter] = false;
                else binaryNumber[^counter] = true;

                number = (uint)(number / 2);
                counter++;
            }

            return binaryNumber;
        }
        public static uint ConvertBinaryToDecimal(bool[] binaryNumber)
        {
            uint decimalNumber = 0, counter = 1;

            for (int i = 1; i <= binaryNumber.Length; i++)
            {
                decimalNumber += i > 1 ? Convert.ToByte(binaryNumber[^i]) * counter : Convert.ToByte(binaryNumber[^i]) * counter;
                counter *= 2;
            }

            return decimalNumber;
        }

        public static bool[][] CreateKeys(bool[] mainKey)
        {
            bool[][] KeySet = new bool[8][]
            {
                new bool[4],
                new bool[4],
                new bool[4],
                new bool[4],
                new bool[4],
                new bool[4],
                new bool[4],
                new bool[4]
            };
            uint mainCounter = 0;

            uint keySetCounter = 0;

            while (mainCounter < 4)
            {
                bool[] leftSide = mainKey[..4];
                bool[] rightSide = mainKey[4..];

                var firstElement = leftSide[0];
                Array.Copy(leftSide, 1, leftSide, 0, leftSide.Length - 1);
                leftSide[^1] = firstElement;

                firstElement = rightSide[0];
                Array.Copy(rightSide, 1, rightSide, 0, rightSide.Length - 1);
                rightSide[^1] = firstElement;

                mainKey = leftSide.Concat(rightSide).ToArray();

                uint differenceCounter = 0;

                for (int i = 0; i < mainKey.Length; i+=2)
                {
                    KeySet[keySetCounter][i - differenceCounter] = mainKey[i];
                    differenceCounter++;
                }

                keySetCounter++;

                firstElement = mainKey[0];
                Array.Copy(mainKey, 1, mainKey, 0, mainKey.Length - 1);
                mainKey[^1] = firstElement;

                differenceCounter = 0;

                for (int i = 0; i < mainKey.Length; i += 2)
                {
                    KeySet[keySetCounter][i - differenceCounter] = mainKey[i];
                    differenceCounter++;
                }

                keySetCounter++;
                mainCounter++;
            }

            return KeySet;
        }

        public static bool[] ArraysGateXOR(bool[] array1, bool[] array2)
        {
            bool[] returnedArray = new bool[4];

            for (int i = array1.Length - 1; i >= 0; i--)
            {
                returnedArray[i] = (array1[i] == true && array2[i] == false) || (array1[i] == false && array2[i] == true) ? true : false;
            }

            return returnedArray;
        }

        public static void FeistelPermutationIteration(ref bool[] leftSide, ref bool[] rightSide, bool[] key)
        {
            bool[] rightAfterSBlock = new bool[4];

            rightAfterSBlock[0] = rightSide[0] ^ (rightSide[0] && rightSide[2]) ^ (rightSide[1] && rightSide[3]) ^(rightSide[1] && rightSide[2] && rightSide[3]) ^ (rightSide[0] && rightSide[1] && rightSide[2] && rightSide[3]) ^ key[0];

            rightAfterSBlock[1] = rightSide[1] ^ (rightSide[0] && rightSide[2]) ^ (rightSide[0] && rightSide[1] && rightSide[3]) ^ (rightSide[0] && rightSide[2] && rightSide[3]) ^(rightSide[0] && rightSide[1] && rightSide[2] && rightSide[3]) ^ key[1];

            rightAfterSBlock[2] = true ^ rightSide[2] ^ (rightSide[0] && rightSide[3]) ^ (rightSide[0] && rightSide[1] && rightSide[3]) ^ (rightSide[0] && rightSide[1] && rightSide[2] && rightSide[3]) ^ key[2];

            rightAfterSBlock[3] = true ^ (rightSide[0] && rightSide[1]) ^ (rightSide[2] && rightSide[3]) ^ (rightSide[0] && rightSide[1] && rightSide[3]) ^ (rightSide[0] && rightSide[2] && rightSide[3]) ^ (rightSide[0] && rightSide[1] && rightSide[2] && rightSide[3]) ^ key[3];

            leftSide = ArraysGateXOR(leftSide, rightAfterSBlock);
        }
        public static bool[] FeistelsPermutation(uint message, uint mainKey, uint option)
        {
            var binaryMessage = ConvertDecimalToBinary(message);
            var binaryKey = ConvertDecimalToBinary(mainKey);
            var keys = CreateKeys(binaryKey);

            var leftSideMessage = binaryMessage[..4];
            var rightSideMessage = binaryMessage[4..];

            if (option == (uint)Option.Encrypt)
            {
                int i = 0;
                while (i < 8)
                {
                    FeistelPermutationIteration(ref leftSideMessage, ref rightSideMessage, keys[i]);
                    i++;
                    FeistelPermutationIteration(ref rightSideMessage, ref leftSideMessage, keys[i]);
                    i++;
                }
            }
            else if (option == (uint)Option.Decrypt)
            {
                int i = 7;
                while (i >= 0)
                {
                    FeistelPermutationIteration(ref leftSideMessage, ref rightSideMessage, keys[i]);
                    i--;
                    FeistelPermutationIteration(ref rightSideMessage, ref leftSideMessage, keys[i]);
                    i--;
                }
            }
            binaryMessage = rightSideMessage.Concat(leftSideMessage).ToArray();


            return binaryMessage;
        }
    }
}
