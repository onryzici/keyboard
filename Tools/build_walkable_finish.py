"""Wall and ceiling finish only; existing wide workshop layout remains intact."""
import os
source=os.path.join(os.path.dirname(__file__),'build_walkable_workshop.py')
exec(open(source,encoding='utf-8').read().split('# Architectural shell:')[0])
OUT=os.path.join(ROOT,'LittleSwitch','Assets','WorkshopRevision','Models')
for i in range(34):
 x=-5.28+i*.32
 box('Timber ceiling lining',(x,3.60,0),(.312,.035,8.25),'HoneyWood',.005)
for x in [-5.32,5.32]:
 for i in range(46):
  z=-4.04+i*.18
  box('Wall timber lower panel',(x,.61,z),(.034,.82,.173),'HoneyWood',.005)
 box('Wall dado cap',(x,1.04,0),(.075,.055,8.25),'DarkWood',.008)
for i in range(59):
 x=-5.22+i*.18
 box('Rear wall timber lower panel',(x,.61,4.14),(.173,.82,.034),'HoneyWood',.005)
 if x< -4.1 or x> -2.69:box('Entrance timber lower panel',(x,.61,-4.14),(.173,.82,.034),'HoneyWood',.005)
export('WideWorkshopWallFinish')
