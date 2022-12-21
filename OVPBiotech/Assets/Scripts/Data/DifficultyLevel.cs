using Firebase.Firestore;
namespace OVPBiotechSpace
{
    [FirestoreData]
    public class DifficultyLevel
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string name { get; set; }
        [FirestoreProperty]
        public int max { get; set; }
        [FirestoreProperty]
        public int min { get; set; }        
    }
}

