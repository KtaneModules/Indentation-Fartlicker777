using System.Collections;
using UnityEngine;

public class Indentation : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;
   public MeshRenderer[] Spheres;
   public KMSelectable[] Selectables;
   public GameObject Circle;
   public MeshRenderer TP;
   public Material TPColor;

   int[][] Distances = new int[][] {
      new int[3] {2, 2, 2},
      new int[2] {3, 1},
      new int[3] {1, 2, 2},
      new int[2] {2, 1},
      new int[2] {1, 2},
      new int[3] {1, 3, 2},
      new int[3] {4, 1, 1},
      new int[2] {2, 3},
      new int[1] {1},
      new int[2] {4, 2},
      new int[3] {1, 1, 3},
      new int[3] {1, 1, 1}
    };
   int[] Answers = { -111, 2, 6, 4, 2, 0, 3, 5, 1, 5, 1, 1 };
   int Sequence;

   static int moduleIdCounter = 1;
   int moduleId;
   private bool moduleSolved;

#pragma warning disable 0649
   bool TwitchPlaysActive;
#pragma warning restore 0649

   void Awake () {
      moduleId = moduleIdCounter++;

      foreach (KMSelectable Selectable in Selectables) {
         Selectable.OnInteract += delegate () { SelectablePress(Selectable); return false; };
      }
      GetComponent<KMBombModule>().OnActivate += FatNutsTP;
   }

   void Start () {
      Sequence = UnityEngine.Random.Range(0, 12);
      Debug.LogFormat("[Indentation #{0}] The selected sequence is {1}.", moduleId, Sequence + 1);
      for (int i = 1; i < 8; i++) {
         Spheres[i].GetComponent<MeshRenderer>().enabled = false;
      }
      int Temp = 0;
      for (int i = 0; i < Distances[Sequence].Length; i++) {
         Temp += Distances[Sequence][i];
         Spheres[Temp % 8].GetComponent<MeshRenderer>().enabled = true;
      }
      StartCoroutine(Spinner());
   }

   void FatNutsTP () {
      if (TwitchPlaysActive) {
         TP.GetComponent<MeshRenderer>().material = TPColor;
      }
   }

   IEnumerator Spinner () {
      bool Direction = UnityEngine.Random.Range(0, 2) == 1 ? true : false;
      while (true) {
         Circle.transform.Rotate(0, Direction ? UnityEngine.Random.Range(0, 2) == 0 ? 1f : 2f : UnityEngine.Random.Range(0, 2) == 0 ? -1f : -2f, 0);
         yield return new WaitForSecondsRealtime(.01f);
      }
   }

   void SelectablePress (KMSelectable Selectable) {
      Selectable.AddInteractionPunch(.1f);
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Selectable.transform);
      if (moduleSolved) {
         return;
      }
      if (Sequence != 0) {
         if (Selectable == Selectables[Answers[Sequence]]) {
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
         }
         else {
            GetComponent<KMBombModule>().HandleStrike();
         }
      }
      else {
         if (Selectable == Selectables[0] || Selectable == Selectables[2] || Selectable == Selectables[4] || Selectable == Selectables[6]) {
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
         }
         else {
            GetComponent<KMBombModule>().HandleStrike();
         }
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} *Abbrievation of cardinal* to press that spot, with the colored sphere being north.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      Command = Command.ToUpper().Trim();
      yield return null;
      string[] Cardinals = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
      for (int i = 0; i < 8; i++) {
         if (Command == Cardinals[i]) {
            Selectables[i].OnInteract();
         }
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      if (Sequence != 0) {
         Selectables[Answers[Sequence]].OnInteract();
      }
      else {
         Selectables[0].OnInteract();
      }
      yield return null;
   }
}
