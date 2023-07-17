using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interactables
{
    public class InteractableObject : MonoBehaviour
    {
        private TypeOfInteractableObject _type;
        public TypeOfInteractableObject Type { get { return _type; } }

        //add already a check on awake for the assigned type and assign the 
        //relevant script reference (Bot/pushableBox)
    }
    public enum TypeOfInteractableObject
    {
        Bot,
        PushableBox,
        PushableBot
    }
}
