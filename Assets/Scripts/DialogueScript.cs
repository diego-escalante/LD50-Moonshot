using UnityEngine;

[System.Serializable]
public class DialogueScript {

    [SerializeField]
    private Speaker[] speakers = {new Speaker(30, Color.white, 0.025f, 1, 0.5f, null, ProfileLocation.Left, Color.white, null)};
    [SerializeField]    
    private SubDialogue[] subDialogues = {new SubDialogue(0, "Hello world!")};

    // GetSubDialogues takes each subdialogue and attaches the correct speaker to it from Speakers 
    // based on the provided SpeakerIndex. This can be improved by only doing it once per GetSubDialogues call,
    // and also when a value is changed in the inspector.
    public SubDialogue[] GetSubDialogues() {
        // Validate Speakers
        for (int i = 0; i < speakers.Length; i++) {
            if (!validateSpeaker(speakers[i])) {
                Debug.LogError(string.Format("Speaker at index {0} is invalid!", i));
            }
        }

        // Attach needed Speaker
        for (int i = 0; i < subDialogues.Length; i++) {
            int j = subDialogues[i].SpeakerIndex;
            if (j >= 0 && j < speakers.Length) {
                subDialogues[i].Speaker = speakers[j];
            } else {
                Debug.LogError(string.Format("SubDialogue at index {0} has a SpeakerIndex {1} outside of the Speakers range!", i, j));
            }
        }

        return subDialogues;
    }

    private bool validateSpeaker(Speaker speaker) {
        return !(speaker.fontSize <= 0 || speaker.scrollInterval < 0 || speaker.pitch <= 0 || speaker.pitchDelta < 0);
    }

    [System.Serializable]
    public struct SubDialogue {
        [SerializeField]
        private int speakerIndex;
        public int SpeakerIndex {get{return speakerIndex;}}

        [SerializeField]
        [TextArea(3, 10)]
        private string text;
        public string Text {get{return text;}}

        public Speaker Speaker {get; internal set;}

        public SubDialogue(int speakerIndex, string text) {
            this.speakerIndex = speakerIndex;
            this.text = text;
            this.Speaker = new Speaker();
        }
    }

    [System.Serializable]
    public struct Speaker {
        public int fontSize;
        public Color fontColor;
        public float scrollInterval;
        public Sprite profile;
        public ProfileLocation profileLocation;
        public Color profileColor;
        public AudioClip scrollSound;
        public float pitch;
        public float pitchDelta;

        public Speaker(
            int fontSize,
            Color fontColor,
            float scrollInterval,
            float pitch,
            float pitchDelta,
            Sprite profile,
            ProfileLocation profileLocation,
            Color profileColor,
            AudioClip scrollSound) {
            
            this.fontSize = fontSize;
            this.fontColor = fontColor;
            this.scrollInterval = scrollInterval;
            this.pitch = pitch;
            this.pitchDelta = pitchDelta;
            this.profile = profile;
            this.profileColor = profileColor;
            this.profileLocation = profileLocation;
            this.scrollSound = scrollSound;
        }
    }

    [System.Serializable]
    public enum ProfileLocation {
        Left,
        Right
    }
}
