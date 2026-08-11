using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyDialogs : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private List<string> dialogs;
    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private GameObject interactionCanvas;

    private int _dialogIndex = 0;
    private int _dialogLength = 0;

    public void NextDialog()
    {
        interactionCanvas.SetActive(false);

        if (!dialogCanvas.activeInHierarchy)
            dialogCanvas.SetActive(true);

        if (_dialogIndex < dialogs.Count)
        {
            dialogText.maxVisibleCharacters = 0;

            var dialogLine = dialogs[_dialogIndex];
            _dialogLength = dialogLine.Length;

            dialogText.text = dialogLine;

            StartCoroutine(TextAnimation());

            _dialogIndex += 1;
        }
        else
        {
            //print("Llegue al final");
            dialogCanvas.SetActive(false);
            _dialogIndex = 0;
        }
    }

    private IEnumerator TextAnimation()
    {
        print("Hola");
        yield return new WaitForSeconds(3.0f);
        print("Chau");
    }
}
