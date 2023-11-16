using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.IO;

//YOUR LOGIC IF FUCKED!!!!!!
//null letter null null letter letter drop letter null is busted

public class Manager_Game : MonoBehaviour
{
    [SerializeField] private bool DeleteOnDrop = false; //Delete me and refactor code when you decide best mechanic

    [SerializeField] private GameObject LetterBlockPrefab;
    [SerializeField] private TMP_Text WordTextList;
    [SerializeField] private TMP_Text ScoreText;
    public GameObject currentLetterBlock;

    [SerializeField] private TextAsset WordListTextAsset;
    [SerializeField] private static HashSet<string> ValidWords;

    [SerializeField] private Manager_Grid manager_Grid;

    private List<Grid_Cell> highlightedCells = new List<Grid_Cell>();
    private List<Word> Words = new List<Word>();
    //private Text textMesh;
    private int currentPosition_Arr = 0;
    private int midPosition_Arr = 0;
    private int score = 0;

    public static IntEvent MoveLetterEvent = new IntEvent();
    public static UnityEvent DropLetterEvent = new UnityEvent();
    public static LetterEvent LetterClickedEvent = new LetterEvent();
    //private static Random _random = new Random();

    // Start is called before the first frame update
    void Start()
    {
        //textMesh = tmProObj.GetComponent<Text>();
        UpdateScore(0);
        MoveLetterEvent.AddListener(MoveLetterBlock);
        DropLetterEvent.AddListener(DropLetterBlock);
        LetterClickedEvent.AddListener(LetterClicked);
        ValidWords = new HashSet<string>();
        StartGame();

    }

    void StartGame()
    {
        LoadWordsFromTextAsset();
        manager_Grid.GridSetUp();

        midPosition_Arr = manager_Grid.AboveGridBlockPositions.Length / 2;

        SpawnNewLetterBlock();
    }

    private void LoadWordsFromTextAsset()
    {
        string[] words = WordListTextAsset.text.Split('\n');
        //ONLY ONE TIME. DONT SHIP
        //FixFile(words);
        //******
        foreach (string word in words)
        {
            ValidWords.Add(word.Trim());
        }
    }
    private static bool IsValidWord(string wordToCheck)
    {
        return ValidWords.Contains(wordToCheck);
    }
    public void SpawnNewLetterBlock()
    {
        currentLetterBlock = Instantiate<GameObject>(LetterBlockPrefab);
        currentLetterBlock.transform.localScale = new Vector3(manager_Grid.xYScaleFactor, manager_Grid.xYScaleFactor, 1);
        currentPosition_Arr = midPosition_Arr;
        currentLetterBlock.GetComponent<Letter_Block>().SetDestination(manager_Grid.AboveGridBlockPositions[midPosition_Arr]);
        currentLetterBlock.transform.position = manager_Grid.AboveGridBlockPositions[midPosition_Arr];
        currentLetterBlock.GetComponent<Letter_Block>().SetLetter(GetRandomLetter());
        HightLight();
    }
    private string GetRandomLetter()
    {
        //What should weights be???? How to Balence? Should it be RNG based on what is on the field?
        //Like if a letter exist on the field already should the chances be lower?


        return GetRandomLetterByFrequency().ToString();
    }
    public void MoveLetterBlock(int direction)
    {
        if (direction < 0 && currentPosition_Arr > 0)
        {
            currentPosition_Arr--;
        }
        else if (direction > 0 && currentPosition_Arr < manager_Grid.AboveGridBlockPositions.Length - 1)
        {
            currentPosition_Arr++;
        }
        //Debug.Log(manager_Grid.AboveGridBlockPositions[currentPosition_Arr] + " pos: " + currentPosition_Arr);
        //currentLetterBlock.transform.position = manager_Grid.AboveGridBlockPositions[currentPosition_Arr];
        currentLetterBlock.GetComponent<Letter_Block>().SetDestination(
            manager_Grid.AboveGridBlockPositions[currentPosition_Arr]);
        HightLight();
    }
    private void HightLight()
    {
        
        foreach(Grid_Cell cell in highlightedCells)
        {
            cell.UnHighlightSpace();  
        }
        highlightedCells.Clear();

        bool isBottom = true;
        for(int i = manager_Grid.Hight - 1; i >= 0; i--)
        {
            Grid_Cell cell = manager_Grid.grid[currentPosition_Arr, i].GetComponent<Grid_Cell>();
            if ( cell.Contained_Letter_Block == null)
            {
                highlightedCells.Add(cell);
                cell.HighlightSpace(isBottom);
                isBottom = false;
            }
        }
    }
    private void LetterClicked(Letter_Block lb)
    {
        List<Word> wordsToDelete = new List<Word>();
        foreach(Word word in Words)
        {
            if(word.Contains_Letter_Block(lb))
            {
                wordsToDelete.Add(word);
                //I dont break here because the letter can be part of 2 words
            }
            
        }
        bool cellClensed = false;
        foreach (Word word in wordsToDelete)
        {
            
            cellClensed = ClenseWordFromClick(word);
            
        }
        if (cellClensed)
        {
            Words.Clear();
            manager_Grid.updateGrid();
            searchWholeGridForBiggestWords();
        }
    }
    public void DropLetterBlock()
    {
        Grid_Cell destination = null;
        for (int i = manager_Grid.Hight - 1; i >= 0; i--)
        {
            //Debug.Log("i = " + i);
            if (manager_Grid.grid[currentPosition_Arr, i].GetComponent<Grid_Cell>().Contained_Letter_Block != null)
            {
                //Debug.Log("Cell Taken");
                continue;
            }
            else
            {
                destination = manager_Grid.grid[currentPosition_Arr, i].GetComponent<Grid_Cell>();
                //Debug.Log("set destination");
                break;
            }
        }
        if (destination != null)
        {
            currentLetterBlock.GetComponent<Letter_Block>().SetDestination(destination);
            //currentLetterBlock.transform.position = destination.gameObject.transform.position;
            destination.Contained_Letter_Block = currentLetterBlock.GetComponent<Letter_Block>();
            currentLetterBlock = null;
            
            searchWholeGridForBiggestWords();
            
            SpawnNewLetterBlock();

        }

    }

    private void searchWholeGridForBiggestWords()
    {
        List<List<Grid_Cell>> CellsToClense = new List<List<Grid_Cell>>();
        //first look for horizontal
        for(int i = 0; i < manager_Grid.grid.GetLength(1); i++)
        {
            Grid_Cell[] cells = new Grid_Cell[manager_Grid.grid.GetLength(0)];
            for(int j = 0; j < manager_Grid.grid.GetLength(0); j++)
            {
                //Debug.Log(i + " : " + j);
                cells[j] = manager_Grid.grid[j,i].GetComponent<Grid_Cell>();
            }
            List<List<Grid_Cell>> tmpCells = new List<List<Grid_Cell>>();
            tmpCells = AreCellsPartOfWord(cells);

            foreach(List<Grid_Cell> cellsList in tmpCells)
            {
                CellsToClense.Add(cellsList);
            }
        }
        //next look for vertical
        for(int i = 0; i < manager_Grid.grid.GetLength(0); i++)
        {
            Grid_Cell[] cells = new Grid_Cell[manager_Grid.grid.GetLength(1)];
            for(int j = manager_Grid.grid.GetLength(1) - 1; j >= 0 ; j--)
            {
                //Debug.Log(i + " : " + j);
                cells[j] = manager_Grid.grid[i, j].GetComponent<Grid_Cell>();
            }
            List<List<Grid_Cell>> tmpCells = new List<List<Grid_Cell>>();
            tmpCells = AreCellsPartOfWord(cells);

            foreach (List<Grid_Cell> cellsList in tmpCells)
            {
                CellsToClense.Add(cellsList);
            }
            
        }
        //next look for diaganal top right to bottom left?
        //next look for diaganal top left to botom right?

        if(DeleteOnDrop == true)//DELETE THIS IF WHEN DECIDE ON MECHANICS!!!!
        {
            //remove cells that make up words
            bool cellClensed = false;
            foreach (List<Grid_Cell> cellsList in CellsToClense)
            {
                //string debugStr = "";
                foreach (Grid_Cell cell in cellsList)
                {
                    if (cell.Contained_Letter_Block != null)
                    {
                        //debugStr = debugStr + cell.Contained_Letter_Block.Letter;
                        Letter_Block tmp = cell.Contained_Letter_Block;
                        cell.Contained_Letter_Block = null;
                        Destroy(tmp.gameObject);
                        cellClensed = true;
                    }
                }
            }
            manager_Grid.updateGrid();

            if (cellClensed == true)
            {
                searchWholeGridForBiggestWords();
            }
        }
        else//Highlight valid words and add to list. If word deleted and grid adjusted then delete list and rebuild.
        {
            foreach(List<Grid_Cell> cellsList in CellsToClense)
            {
                if(cellsList.Count <= 0)
                {
                    continue;
                }
                Word word = new Word();
                foreach(Grid_Cell cell in cellsList)
                {
                    word.AddLetterToWord(cell.Contained_Letter_Block);
                    cell.Contained_Letter_Block.HighlightLetter();
                }
                Words.Add(word);
            }
        }
    }
    private bool ClenseWordFromClick(Word word)
    {
        bool cellClensed = false;
        foreach(Letter_Block letter_Block in word.letter_Blocks)
        {
            letter_Block.parentCell.Contained_Letter_Block = null;
            Destroy(letter_Block.gameObject);
            cellClensed = true;
        }
        MoveLetterBlock(0);
        return cellClensed;
        
    }
    //if cell clensed then update grid and highlight new cells
    private void ClenseCells(List<Grid_Cell> cells)
    {
        bool cellClensed = false;
        foreach (Grid_Cell cell in cells)
        {
            if (cell.Contained_Letter_Block != null)
            {
                //debugStr = debugStr + cell.Contained_Letter_Block.Letter;
                Letter_Block tmp = cell.Contained_Letter_Block;
                cell.Contained_Letter_Block = null;
                Destroy(tmp.gameObject);
                cellClensed = true;
            }
        }
        if(cellClensed == true)
        {
            manager_Grid.updateGrid();
            searchWholeGridForBiggestWords();
        }
    }
    //
    private List<List<Grid_Cell>> AreCellsPartOfWord(Grid_Cell[] cells)
    {
        List<List<Grid_Cell>> potentialWords = new List<List<Grid_Cell>>();
        int strNum = 0;
        bool letterSeen = false;
        potentialWords.Add(new List<Grid_Cell>());
        foreach (Grid_Cell cell in cells)
        {
            if (cell.Contained_Letter_Block != null)
            {
                potentialWords[strNum].Add(cell);
                letterSeen = true;
            }
            else if (cell.Contained_Letter_Block == null && letterSeen == true)
            {
                potentialWords.Add(new List<Grid_Cell>());
                strNum = strNum + 1;
                letterSeen = false;
            }
        }
        List<List<Grid_Cell>> foundWords = new List<List<Grid_Cell>>();
        foreach(List<Grid_Cell> cellList in potentialWords)
        {
            foundWords.Add(DoesGridCellListContainWord(cellList));
        }
        //Needs to be moved if we go with click solution due to constant score bug
        int scoreToAdd = 0;
        for(int i = 0; i < foundWords.Count; i++)
        {
            scoreToAdd = scoreToAdd + (Scores[foundWords[i].Count] * (i+1));   
        }
        UpdateScore(Scores[foundWords[0].Count]);
        return foundWords;
    }
    private List<Grid_Cell> DoesGridCellListContainWord(List<Grid_Cell> cells)
    {
        string s = "";
        int max = 0;
        List<Grid_Cell> returnCells = new List<Grid_Cell>();
        foreach(Grid_Cell cell in cells)
        {
            s = s + cell.Contained_Letter_Block.Letter;
        }
        for (int length = 1; length <= s.Length; length++)
        {
            for (int start = 0; start <= s.Length - length; start++)
            {
                string substring = s.Substring(start, length);
                if(substring.Length > max && ValidWords.Contains(substring))
                {
                    max = substring.Length;
                    returnCells.Clear();
                    for(int i = start; i < start + length; i++)
                    {
                        //Debug.Log("return Cell: " + cells[i].Contained_Letter_Block.Letter);
                        returnCells.Add(cells[i]);
                    }
                }
            }
        }
        if(returnCells.Count > 0 && DeleteOnDrop == true)
        {
            string word = "";
            foreach (Grid_Cell cell in returnCells)
            {
                word = word + cell.Contained_Letter_Block.Letter;
            }
            WordTextList.text = word + "\n" + WordTextList.text;
            
        }

        return returnCells;
    }
   
    public char GetRandomLetterByFrequency()
    {
        System.Random _random = new System.Random();
        double totalWeight = LetterFrequencies.Values.Sum();
        double randomValue = _random.NextDouble() * totalWeight;

        foreach (var entry in LetterFrequencies)
        {
            if (randomValue < entry.Value)
            {
                return entry.Key;
            }

            randomValue -= entry.Value;
        }
        return 'X';
        //throw new InvalidOperationException("Should not reach this point.");
    }
    private void UpdateScore(int amt)
    {
        score = score + amt;
        ScoreText.text = "Score: " + score;
    }
    private static readonly int[] Scores = new int[]
    {
        0,
        0,
        1,
        2,
        4,
        8,
        16,
        32,
        64,
        128,
        256,
        512,
        1024,
        2048,
        4096,
        8192,
        16384
    };
    private static readonly Dictionary<char, double> LetterFrequencies = new Dictionary<char, double>
    {
        {'E', 13.11},//A
        {'T', 10.56},//B
        {'A', 8.17}, //C
        {'O', 7.63}, //D
        {'I', 6.96}, //E
        {'N', 6.75}, //F
        {'S', 6.33}, //G
        {'H', 6.09}, //H
        {'R', 5.99}, //I
        {'D', 4.25}, //J
        {'L', 4.03}, //K
        {'U', 2.76}, //L
        {'W', 2.36}, //M
        {'M', 2.21}, //N
        {'F', 2.18}, //O
        {'C', 2.02}, //P
        {'G', 2.02}, //Q
        {'Y', 1.97}, //R
        {'P', 1.93}, //S
        {'B', 1.49}, //T
        {'K', 1.29}, //U
        {'V', 0.98}, //V
        {'J', 0.15}, //W
        {'X', 0.15}, //X
        {'Q', 0.10}, //Y
        {'Z', 0.07}  //Z
    };

   

    //REMOVE BEFORE SEND TO PROD
    private void FixFile(string[] words)
    {
        

        // Filter out words that are two characters or less
        var filteredWords = words.Where(word => word.Length > 2);

        // Combine the filtered words back into a single string with a space as separator
        string filteredText = string.Join("\n", filteredWords);

        // Now you have the filtered text. You can use it as needed in your application.
        // For example, you can write it to a new file or use it directly as a string.

        // If you need to write it to a new TextAsset, you'd typically write it to a file:
        WriteTextToFile(filteredText, "FilteredText.txt");
    }
    void WriteTextToFile(string text, string fileName)
    {
        string filePath = Path.Combine("Assets/TextAssets", fileName);

        File.WriteAllText(filePath, text);

        Debug.Log("Filtered text written to " + filePath);
    }

}

[System.Serializable]
public class IntEvent : UnityEvent<int>
{
}
public class LetterEvent : UnityEvent<Letter_Block>
{
}



