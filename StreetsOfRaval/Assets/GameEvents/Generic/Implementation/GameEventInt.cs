using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace streetsofraval { 
//And now we create a new GameEvent that extends from the generic abstract class
//filename = Default file name
//menuName = Name that shows upon right click -> scriptable objects -> ... to create a new
[CreateAssetMenu(fileName = "GameEvent", menuName = "GameEvent/GameEvent - Integer")]
public class GameEventInt : GameEvent<int>
{
}
}
