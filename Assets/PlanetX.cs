using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlanetX : MonoBehaviour {

	public KMBombModule module;
	public new KMAudio audio;
	private int moduleId;
	private static int moduleIdCounter = 1;

	public KMSelectable leftNote;
	public KMSelectable rightNote;
	public KMSelectable upSector;
	public KMSelectable downSector;
	public KMSelectable revealSectorButt;
	public KMSelectable submit;
	public TextMesh revealSectorText;

	public MeshRenderer[] noteScreens;
	public TextMesh[] noteScreenTexts;
	public TextMesh noteScreenPageNum;
	public MeshRenderer sectorScreen;
	public TextMesh sectorText;
	

	private int noteIndex;
	private int sectorIndex;
	private int revealCounter;
	private string board;
	public Material[] mats;
	public AudioClip cycleSFX;
	public AudioClip targetSFX;
	public AudioClip solveSFX;

	private List<NoteDisplayInfo> noteInfos;
	private DisplayInfo[] sectors;

	private bool TPautosolve = false;
	void Awake()
	{
		moduleId = moduleIdCounter++;
		generateModule();
	}
	private void generateModule()
	{
		noteIndex = 0;
		sectorIndex = 0;
		revealCounter = 4;
		Puzzle puzzle = new Puzzle();
		PuzzleInfo puzzleInfo = puzzle.generatePuzzle();
		board = puzzleInfo.getBoard();
		List<NoteObj> notes = puzzleInfo.getNotes();
		notes.Shuffle();
		noteInfos = new List<NoteDisplayInfo>();
		sectors = new DisplayInfo[12];
		for (int i = 0; i < sectors.Length; i++)
			sectors[i] = new DisplayInfo("", 0);
		foreach (NoteObj note in notes)
		{
			string code = note.getCode();
			if (code.EndsWith("T"))
				sectors[Int16.Parse(code.Substring(0, 2)) - 1] = new DisplayInfo("", getMatIndex(code.Substring(2, 2)));
			else
				noteInfos.Add(new NoteDisplayInfo(code));
		}
		revealSectorText.text = revealCounter + "";
		displayNote();
		displaySector();
		printLogs();

		leftNote.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX.name, transform); noteIndex = mod(noteIndex - 1, noteInfos.Count); displayNote(); return false; };
		rightNote.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX.name, transform); noteIndex = mod(noteIndex + 1, noteInfos.Count); displayNote(); return false; };
		upSector.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX.name, transform); sectorIndex = mod(sectorIndex + 1, sectors.Length); displaySector(); return false; };
		downSector.OnInteract = delegate { audio.PlaySoundAtTransform(cycleSFX.name, transform); sectorIndex = mod(sectorIndex - 1, sectors.Length); displaySector(); return false; };
		revealSectorButt.OnInteract = delegate { revealSector(); return false; };
		submit.OnInteract = delegate { submitX(); return false; };
	}
	private void printLogs()
	{
		Debug.LogFormat("[Planet X #{0}] Galaxy Configuration", moduleId);
		for (int i = 0; i < board.Length; i++)
			Debug.LogFormat("[Planet X #{0}] Sector #{1}: {2} {3}", moduleId, i + 1, charToStr(board[i], true), sectors[i].getMatIndex() > 0 ? "(" + convertPlanet(sectors[i].getMatIndex()) + ")" : "");
		Debug.LogFormat("[Planet X #{0}] Notes Generated", moduleId);
		for(int i = 0; i < noteInfos.Count; i++)
			Debug.LogFormat("[Planet X #{0}] Note #{1}: {2}", moduleId, i + 1, noteToStr(noteInfos[i]));
	}
	private string noteToStr(NoteDisplayInfo note)
	{
		DisplayInfo[] displayInfos = note.getDisplays();
		string str = "";
		switch(displayInfos[0].getText())
		{
			case "S":
				str = "Sectors " + displayInfos[1].getText() + " - " + displayInfos[2].getText() + " contains " + convertEF(displayInfos[4].getText(), displayInfos[5].getText()) + " " + convertPlanet(displayInfos[3].getMatIndex());
				break;
			case "A":
				str = capital(convertEF(displayInfos[4].getText(), displayInfos[5].getText())) + " " + convertPlanet(displayInfos[1].getMatIndex()) + " is adjacent to a " + convertPlanet(displayInfos[2].getMatIndex()).Replace("(s)", "");
				break;
			case "O":
				str = capital(convertEF(displayInfos[4].getText(), displayInfos[5].getText())) + " " + convertPlanet(displayInfos[1].getMatIndex()) + " is opposite to a " + convertPlanet(displayInfos[2].getMatIndex()).Replace("(s)", "");
				break;
			case "W":
				str = capital(convertEF(displayInfos[4].getText(), displayInfos[5].getText())) + " " + convertPlanet(displayInfos[1].getMatIndex()) + " is within " + displayInfos[3].getText() + " sectors of a " + convertPlanet(displayInfos[2].getMatIndex()).Replace("(s)", "");
				break;
			case "B":
				str = "All " + convertPlanet(displayInfos[1].getMatIndex()) + " are in a band of " + convertEF(displayInfos[4].getText(), displayInfos[5].getText()) + " sectors";
				break;
		}
		return str;
	}
	private string capital(string str)
	{
		string s = str[0] + "";
		s = s.ToUpperInvariant();
		return (s + str.Substring(1));
	}
	private string convertEF(string e, string f)
	{
		switch(e)
		{
			case "E": return "exactly " + f;
			case "L": return "at least " + f;
			default: return f + " or fewer";
		}
	}
	private string convertPlanet(int matIndex)
	{
		switch(matIndex)
		{
			case 1: return "Asteroid(s)";
			case 2: return "Comet(s)";
			case 3: return "Dwarf Planet(s)";
			case 4: return "Gas Cloud(s)";
			case 5: return "Sector(s) that appear empty";
			case 6: return "Planet X(s)";
			case 7: return "No Asteroid";
			case 8: return "No Comet";
			case 9: return "No Dwarf Planet";
			case 10: return "No Gas Cloud";
		}
		return "ERROR";
	}
	private void displaySector()
	{
		sectorText.text = (sectorIndex + 1) + "";
		while (sectorText.text.Length < 2)
			sectorText.text = "0" + sectorText.text;
		sectorScreen.material = mats[sectors[sectorIndex].getMatIndex()];
	}
	private void displayNote()
	{
		noteScreenPageNum.text = (noteIndex + 1) + "";
		DisplayInfo[] displays = noteInfos[noteIndex].getDisplays();
		for(int i = 0; i < displays.Length; i++)
		{
			noteScreens[i].material = mats[displays[i].getMatIndex()];
			noteScreenTexts[i].text = displays[i].getText();
		}
	}
	private void revealSector()
	{
		if(revealCounter > 0 && !(sectors[sectorIndex].getMatIndex() >= 1 && sectors[sectorIndex].getMatIndex() <= 5))
		{
			audio.PlaySoundAtTransform(targetSFX.name, transform);
			revealCounter--;
			revealSectorText.text = revealCounter + "";
			string temp = board.Replace("X", "E");
			sectors[sectorIndex] = new DisplayInfo("", getMatIndex(temp[sectorIndex] + ""));
			displaySector();
			Debug.LogFormat("[Planet X #{0}] User is revealing Sector #{1}: {2}", moduleId, sectorIndex + 1, charToStr(temp[sectorIndex], false));
		}
		
	}
	private void submitX()
	{
		Debug.LogFormat("[Planet X #{0}] User submitted Sector #{1}", moduleId, sectorIndex + 1);
		if (board[sectorIndex] == 'X')
		{
			audio.PlaySoundAtTransform(solveSFX.name, transform);
			leftNote.OnInteract = null;
			rightNote.OnInteract = null;
			upSector.OnInteract = null;
			downSector.OnInteract = null;
			revealSectorButt.OnInteract = null;
			submit.OnInteract = null;
			foreach(MeshRenderer noteScreen in noteScreens)
				noteScreen.material = mats[0];
			foreach (TextMesh noteText in noteScreenTexts)
				noteText.text = "";
			noteScreenPageNum.text = "";
			revealSectorText.text = "";
			sectorScreen.material = mats[6];
			module.HandlePass();
		}
		else
		{
			module.HandleStrike();
			if (revealCounter > 0)
				revealCounter--;
			revealSectorText.text = revealCounter + "";
			sectors[sectorIndex] = new DisplayInfo("", getMatIndex(board[sectorIndex] + ""));
			displaySector();
			string temp = board.Replace("X", "E");
			Debug.LogFormat("[Planet X #{0}] User recieved a strike! Revealing Sector #{1}: {2}", moduleId, sectorIndex + 1, charToStr(temp[sectorIndex], false));
		}
	}
	private int getMatIndex(string str)
	{
		switch(str)
		{
			case "A": return 1;
			case "AO": return 1;
			case "C": return 2;
			case "CO": return 2;
			case "D": return 3;
			case "DO": return 3;
			case "G": return 4;
			case "GO": return 4;
			case "E": return 5;
			case "EO": return 5;
			case "AX": return 7;
			case "CX": return 8;
			case "DX": return 9;
			case "GX": return 10;
		}
		return -1;
	}
	
	private int mod(int n, int m)
	{
		while (n < m)
			n += m;
		return (n % m);
	}
	private string charToStr(char p, bool revealX)
	{
		switch(p)
		{
			case 'A': return "Asteroid";
			case 'C': return "Comet";
			case 'D': return "Dwarf Planet";
			case 'G': return "Gas Cloud";
			default:
				if (revealX)
				{
					if (p == 'X')
						return "Planet X";
					return "Truly Empty";
				}
				else
					return "Appears Empty";
		}
	}

#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} press|p (SU)p (SD)own (NR)ight (NL)eft (R)eveal (SUB)mit to press those buttons on the module. !{0} cycle|c (N)otes/(S)ectors to cycle the notes/sectors. !{0} reveal|r # to reveal the sector you input. !{0} submit|sub # to submit the sector you input.";
#pragma warning restore 414
	IEnumerator ProcessTwitchCommand(string command)
	{
		if (!TPautosolve)
		{
			string[] param = command.ToUpper().Split(' ');
			if ((Regex.IsMatch(param[0], @"^\s*PRESS\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(param[0], @"^\s*P\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) && param.Length > 1)
			{
				if (isButton(param))
				{
					yield return null;
					for (int i = 1; i < param.Length; i++)
					{
						switch (param[i])
						{
							case "SUP":
							case "SU":
								upSector.OnInteract();
								break;
							case "SDOWN":
							case "SD":
								downSector.OnInteract();
								break;
							case "NRIGHT":
							case "NR":
								rightNote.OnInteract();
								break;
							case "NLEFT":
							case "NL":
								leftNote.OnInteract();
								break;
							case "REVEAL":
							case "R":
								revealSectorButt.OnInteract();
								break;
							case "SUBMIT":
							case "SUB":
								submit.OnInteract();
								break;
						}
						yield return new WaitForSeconds(0.2f);
					}
				}
				else
					yield return "sendtochat An error occured because the user inputted something wrong.";
			}
			else if ((Regex.IsMatch(param[0], @"^\s*CYCLE\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(param[0], @"^\s*C\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) && param.Length == 2 && isCycable(param[1]))
			{
				yield return null;
				if (param[1].Equals("NOTES") || param[1].Equals("N"))
				{
					while(noteIndex != 0)
					{
						rightNote.OnInteract();
						yield return new WaitForSeconds(0.1f);
					}
					for (int i = 0; i < noteInfos.Count; i++)
					{
						yield return new WaitForSeconds(5f);
						rightNote.OnInteract();
					}
				}
				else
				{
					while(sectorIndex != 0)
					{
						upSector.OnInteract();
						yield return new WaitForSeconds(0.1f);
					}
					for (int i = 0; i < sectors.Length; i++)
					{
						yield return new WaitForSeconds(1f);
						upSector.OnInteract();
					}
				}
			}
			else if ((Regex.IsMatch(param[0], @"^\s*REVEAL\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(param[0], @"^\s*R\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) && param.Length == 2 && isNum(param[1]))
			{
				yield return null;
				int target = Int16.Parse(param[1]) - 1;
				while (sectorIndex != target)
				{
					upSector.OnInteract();
					yield return new WaitForSeconds(0.1f);
				}
				yield return new WaitForSeconds(0.2f);
				revealSectorButt.OnInteract();
			}
			else if ((Regex.IsMatch(param[0], @"^\s*SUBMIT\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) || Regex.IsMatch(param[0], @"^\s*SUB\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)) && param.Length == 2 && isNum(param[1]))
			{
				yield return null;
				int target = Int16.Parse(param[1]) - 1;
				while(sectorIndex != target)
				{
					upSector.OnInteract();
					yield return new WaitForSeconds(0.1f);
				}
				yield return new WaitForSeconds(0.2f);
				submit.OnInteract();
			}
			else
				yield return "sendtochat An error occured because the user inputted something wrong.";
		}
		else
			yield return "sendtochat Module is being solved at the moment.";
	}

	private bool isButton(string[] param)
	{
		for (int i = 1; i < param.Length; i++)
		{
			switch (param[i])
			{
				case "SUP":
				case "SU":
				case "NRIGHT":
				case "NR":
				case "SDOWN":
				case "SD":
				case "NLEFT":
				case "NL":
				case "REVEAL":
				case "R":
				case "SUBMIT":
				case "SUB":
					break;
				default:
					return false;
			}
		}
		return true;
	}
	private bool isCycable(string s)
	{
		switch(s)
		{
			case "NOTES":
			case "N":
			case "SECTORS":
			case "S":
				return true;
			default:
				return false;
		}
	}
	private bool isNum(string s)
	{
		switch(s)
		{
			case "1":
			case "2":
			case "3":
			case "4":
			case "5":
			case "6":
			case "7":
			case "8":
			case "9":
			case "10":
			case "11":
			case "12":
				return true;
			default:
				return false;
		}
	}
	private IEnumerator TwitchHandleForcedSolve()
	{
		TPautosolve = true;
		int targetSector = board.IndexOf('X');
		while(targetSector != sectorIndex)
		{
			upSector.OnInteract();
			yield return new WaitForSeconds(0.1f);
		}
		submit.OnInteract();
		yield return new WaitForSeconds(0.1f);
	}
}
