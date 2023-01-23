using Firebase.Firestore;
namespace OVPBiotechSpace
{
    [FirestoreData]
    public struct PostChange
    {        
        [FirestoreProperty]
        public Timestamp date_publish { get; set; }       
    }    
}
