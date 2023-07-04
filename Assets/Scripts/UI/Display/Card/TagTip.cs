using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TagTip : MonoBehaviour
{    [SerializeField] private TextMeshProUGUI tagName;
     [SerializeField] private TextMeshProUGUI tagDescription;
 
     public bool UpdateTagTip(Tag tag)
     {
         if (tag.tagDescription == "")
         {
             gameObject.SetActive(false);
             return false;
         }
 
         gameObject.SetActive(true);
         tagName.text = tag.tagName;
         tagDescription.text = tag.tagDescription;
         return true;
     }
 
     public void Hide()
     {
         gameObject.SetActive(false);
     }
}
