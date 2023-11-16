using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word
{
    private List<Letter_Block> Letter_Blocks = new List<Letter_Block>();

    public List<Letter_Block> letter_Blocks { get => Letter_Blocks; }

    public void AddLetterToWord(Letter_Block letter)
    {
        Letter_Blocks.Add(letter);
    }
    public bool Contains_Letter_Block(Letter_Block letter)
    {
        return Letter_Blocks.Contains(letter);
    }
}
