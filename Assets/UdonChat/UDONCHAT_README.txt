UDONCHAT PRE-RELEASE VERSION 1.4.0

CHANGELIST
1. UdonStringEvents is dramatically more performant now.
2. UdonKeyboard is dramatically more performant now.
3. UdonChat can now be put into static mode.
4. UdonChat hangs off of the player's right-hand tracker rather than their right hand bone, making things far more stable.
5. UdonChat's VR buttons have been changed. To use a button, hover over it with your left index finger and hit your left trigger button.

NOTES:

TO ENABLE STATIC MODE, select the Logger, Keyboard and Buttons objects within the UdonChat prefab whilst its in the scene and set their Udon Behaviours' "Is Static Screen" checkmarks to true.

TO OPTIMISE FOR PERFORMANCE: Go into the UdonChat prefab whilst it's in the scene, go to Udon Event System, EventReceiver, and inside there is a long list of EventEmitters. You only need as many emitters
as is equal to two times your world's soft cap, plus one. For example, the default VRChat world holds 16 people, so two times sixteen plus one is 33, and in this case you'd only need 33 EventEmitters
inside the EventReceiver object. BY DEFAULT UDONCHAT COMES WITH 81 EVENTEMITTERS. THIS IS FOR THE WORST-CASE SCENARIO USING THE WORLD WITH A SOFT CAP OF 40.