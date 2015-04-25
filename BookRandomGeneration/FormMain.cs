using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using BookRandomGeneration.Properties;

namespace BookRandomGeneration
{
  public partial class FormMain : Form
  {
    public FormMain()
    {
      InitializeComponent();
    }

    private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AboutBoxApplication aboutBoxApplication = new AboutBoxApplication();
      aboutBoxApplication.ShowDialog();
    }

    private void DisplayTitle()
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      Text += string.Format(" V{0}.{1}.{2}.{3}", fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart, fvi.FilePrivatePart);
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
      DisplayTitle();
      GetWindowValue();
      FillComboBoxLanguage(comboBoxLanguages);
    }

    private static void FillComboBoxLanguage(ComboBox cb)
    {
      cb.Items.Clear();
      cb.Items.Add("French");
      cb.Items.Add("English");
      cb.SelectedIndex = 0;
    }

    private void GetWindowValue()
    {
      Width = Settings.Default.WindowWidth;
      Height = Settings.Default.WindowHeight;
      Top = Settings.Default.WindowTop < 0 ? 0 : Settings.Default.WindowTop;
      Left = Settings.Default.WindowLeft < 0 ? 0 : Settings.Default.WindowLeft;
    }

    private void SaveWindowValue()
    {
      Settings.Default.WindowHeight = Height;
      Settings.Default.WindowWidth = Width;
      Settings.Default.WindowLeft = Left;
      Settings.Default.WindowTop = Top;
      Settings.Default.Save();
    }

    private void FormMainFormClosing(object sender, FormClosingEventArgs e)
    {
      SaveWindowValue();
    }

    private void buttonGenerateRandomText_Click(object sender, EventArgs e)
    {
      // load dictionary file
      // create random words with punctuation

    }

    public static string GenerateRandomLongString(char[] forbiddenCharacters,
      bool hasForbiddenCharacters = false,
      RandomCharacters rdnCharacters = RandomCharacters.LowerCase, int length = 8,
      bool isWindowsFileName = false)
    {
      if (length < byte.MaxValue)
      {
        return GenerateRandomString(forbiddenCharacters, hasForbiddenCharacters, rdnCharacters, (byte)length, isWindowsFileName);
      }

      string result = string.Empty;
      int leftOver = length % 254;
      for (int i = 1; i <= Math.Floor((decimal)(length / 254)); i++)
      {
        result += GenerateRandomString(forbiddenCharacters, hasForbiddenCharacters, rdnCharacters, 254, isWindowsFileName);
      }

      if (leftOver != 0)
      {
        result += GenerateRandomString(forbiddenCharacters, hasForbiddenCharacters, rdnCharacters, (byte)leftOver, isWindowsFileName);
      }

      return result;
    }

    public static string GenerateRandomString(char[] forbiddenCharacters,
       bool hasForbiddenCharacters = false,
       RandomCharacters rdnCharacters = RandomCharacters.LowerCase, byte length = 8,
       bool isWindowsFileName = false)
    {
      if (length > byte.MaxValue)
      {
        length = byte.MaxValue;
      }

      char[] forbiddenWindowsFilenameCharacters = { '\\', '/', ':', '*', '?', '\"', '<', '>', '|' };
      char[] upperCaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
      char[] lowerCaseCharacters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
      char[] digitCharacters = "0123456789".ToCharArray();
      char[] specialCharacters = ",.;:?!/@#$%^&()=+*-_{}[]|~".ToCharArray();
      char[] searchedCharacters = new char[26 + 26 + 10 + 26]; // max size

      // int numberOfCharactersToPickFrom = (int)rdnCharacters;
      if (isWindowsFileName)
      {
        forbiddenCharacters = AddCharArray(forbiddenCharacters, forbiddenWindowsFilenameCharacters, new[] { ' ' });
        char[] badWindowsFileName = { ',', '!', '.', ';', '@', '#', '$', '%', '^', '&', '(', ')', '=', '+', '{', '}', '~' };
        forbiddenCharacters = AddCharArray(forbiddenCharacters, badWindowsFileName, new[] { ' ' });
      }

      switch (rdnCharacters)
      {
        case RandomCharacters.LowerCase:
          searchedCharacters = FillSearchedCharWithoutForbiddenChar(lowerCaseCharacters, forbiddenCharacters).ToCharArray();
          break;
        case RandomCharacters.UpperCase:
          searchedCharacters = FillSearchedCharWithoutForbiddenChar(upperCaseCharacters, forbiddenCharacters).ToCharArray();
          break;
        case RandomCharacters.Digit:
          searchedCharacters = FillSearchedCharWithoutForbiddenChar(digitCharacters, forbiddenCharacters).ToCharArray();
          break;
        case RandomCharacters.SpecialCharacter:
          searchedCharacters = FillSearchedCharWithoutForbiddenChar(specialCharacters, forbiddenCharacters).ToCharArray();
          break;
        case RandomCharacters.UpperLower:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(upperCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(lowerCaseCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.DigitSpecialChar:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(digitCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(specialCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.LowerDigit:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(lowerCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(digitCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.LowerSpecialChar:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(specialCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(lowerCaseCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.UpperDigit:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(upperCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(digitCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.UpperLowerDigit:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(upperCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(lowerCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(digitCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.UpperSpecialChar:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(upperCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(specialCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.UpperLowerSpecial:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(upperCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(lowerCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(specialCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.UpperDigitSpecial:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(upperCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(specialCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(digitCharacters, forbiddenCharacters)).ToCharArray();
          break;
        case RandomCharacters.UpperLowerDigitSpecial:
          searchedCharacters = (FillSearchedCharWithoutForbiddenChar(upperCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(lowerCaseCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(digitCharacters, forbiddenCharacters) +
            FillSearchedCharWithoutForbiddenChar(specialCharacters, forbiddenCharacters)).ToCharArray();
          break;

        default:
          searchedCharacters = FillSearchedCharWithoutForbiddenChar(lowerCaseCharacters, forbiddenCharacters).ToCharArray();
          break;
      }

      // once we have the SearchedCharacters filled out, we can select random characters from it
      string result = string.Empty;
      for (int i = 0; i < length; i++)
      {
        result += searchedCharacters[GenerateRandomNumberUsingCrypto(0, searchedCharacters.Length - 1)];
      }

      return result;
    }

    public static int GenerateRandomNumberUsingCrypto(int min, int max)
    {
      if (max >= 255)
      {
        return 0;
      }
      int result;
      var crypto = new RNGCryptoServiceProvider();
      byte[] randomNumber = new byte[1];
      do
      {
        crypto.GetBytes(randomNumber);
        result = randomNumber[0];
      } while (result <= min || result >= max);

      return result;
    }
    
    public static string FillSearchedCharWithoutForbiddenChar(char[] source, char[] forbiddenCharacters)
    {
      string result = string.Empty;
      foreach (char item in source)
      {
        if (forbiddenCharacters != null)
        {
          if (!forbiddenCharacters.Contains(item))
          {
            result += item.ToString();
          }
        }
        else
        {
          result += item.ToString();
        }
      }

      return result;
    }

    public static char[] AddCharArray(char[] source, char[] toBeAdded, char[] forbiddenCharacters)
    {
      List<char> tmpSource = source.ToList();
      List<char> tmpToBeAdded = toBeAdded.ToList();
      List<char> tmpforbiddenChar = forbiddenCharacters.ToList();
      foreach (char item in tmpToBeAdded)
      {
        if (!tmpforbiddenChar.Contains(item))
        {
          tmpSource.Add(item);
        }
      }

      return tmpSource.ToArray();
    }

    public enum RandomCharacters
    {
      LowerCase = 1,
      UpperCase = 2,
      Digit = 3,
      SpecialCharacter = 4,
      UpperLower = 5, //LowerCase & UpperCase,
      LowerDigit = 6, //LowerCase & Digit,
      UpperDigit = 7, //UpperCase & Digit,
      UpperLowerDigit = 8, //UpperLower & Digit,
      LowerSpecialChar = 9, //LowerCase & SpecialCharacter,
      UpperSpecialChar = 10, //UpperCase & SpecialCharacter,
      DigitSpecialChar = 11, //Digit & SpecialCharacter
      UpperLowerSpecial = 12,
      UpperDigitSpecial = 13,
      UpperLowerDigitSpecial = 14 // kept numbering because of a possible future change
    }
  }
}