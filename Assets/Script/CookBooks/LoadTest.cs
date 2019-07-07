using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTest : MonoBehaviour {
    public string path = "E:/Projects/UnityProjects/Noob-s-Restaurant-master/Noob-s-Restaurant/Assets/11111.xml";

    // Use this for initialization
    void Start () {
        //CookBooks.CookBookBase book = CookBookTranslate.TranslateToBook(BookAsset);
        //Debug.Log(book.Info.CookBookName);
        //Debug.Log(book.Info.Description);
        var book = CookBookUtilities.CookBookTranslate.LoadCookBookFromXML(path);
        Debug.Log(book.Info.CookBookName);
        Debug.Log(book.Info.Description);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
