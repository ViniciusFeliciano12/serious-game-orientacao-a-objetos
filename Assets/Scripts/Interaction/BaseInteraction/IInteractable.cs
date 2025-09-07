
namespace EJETAGame {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public interface IInteractable
    {
        void Interact(); //Called when we want to interact with the gameobject, eg. when clicking a button;
        void OnInteractEnter(); //Called when detection with the object starts;
        void OnInteractExit();  //Called when detection with the object ends;
    }
}

