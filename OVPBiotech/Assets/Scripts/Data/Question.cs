using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
namespace OVPBiotechSpace
{
    [FirestoreData]
    public struct Question
    {
        [FirestoreProperty]
        public string id { get; set; }
        [FirestoreProperty]
        public string q_question { get; set; }
        [FirestoreProperty]
        public string q_option1 { get; set; }
        [FirestoreProperty]
        public string q_option2 { get; set; }
        [FirestoreProperty]
        public string q_option3 { get; set; }
        [FirestoreProperty]
        public string q_option4 { get; set; }
        [FirestoreProperty]
        public int q_option_correct { get; set; }        
        [FirestoreProperty]
        public string q_category { get; set; }
        [FirestoreProperty]
        public string q_difficulty { get; set; }
        [FirestoreProperty]
        public int q_scaleDifficulty { get; set; }
        [FirestoreProperty]
        public string q_explanation { get; set; }
        [FirestoreProperty]
        public bool published { get; set; }
        [FirestoreProperty]
        public string auth { get; set; }        
    }
}
