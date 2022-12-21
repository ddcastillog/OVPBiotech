using Firebase.Firestore;
namespace OVPBiotechSpace
{
    [FirestoreData]
    public class Category
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string name { get; set; }        
    }
}
