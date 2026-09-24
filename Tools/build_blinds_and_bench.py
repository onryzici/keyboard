"""Original fitted venetian blinds, bench cabinet and functional wire puller."""
import os
exec(open(os.path.join(os.path.dirname(__file__),'build_walkable_workshop.py'),encoding='utf-8').read().split('# Architectural shell:')[0])
OUT=os.path.join(ROOT,'LittleSwitch','Assets','WorkshopRevision','Models')
for centre,width in [(-.64,1.80),(1.7,1.94)]:
 box('Blind headrail',(-5.24,2.79,centre),(.11,.085,width+.06),'Cream',.012)
 for i in range(15):
  slat=box('Tilted blind slat',(-5.24,2.65-i*.092,centre),(.085,.013,width),'Cream',.006)
  slat.rotation_euler.y=math.radians(-23)
 for z in [centre-width*.32,centre+width*.32]:rod('Ladder cord',(-5.19,1.30,z),(-5.19,2.76,z),.004,'Paper')
 rod('Tilt wand',(-5.13,2.70,centre+width*.43),(-5.13,1.91,centre+width*.43),.009,'HoneyWood')
export('FittedVenetianBlinds')
# Fits on the existing lower shelf, doors facing player, preserves knee space at front.
box('Cabinet carcass',(0,.255,0),(1.03,.51,.50),'Metal',.014)
for x in [-.25,.25]:
 box('Recessed cabinet door',(x,.255,-.261),(.485,.475,.035),'Sage',.014)
 box('Inset door panel',(x,.255,-.282),(.41,.39,.011),'Sage',.009)
 rod('Cabinet handle',(x+.14,.23,-.309),(x+.14,.33,-.309),.01,'Steel')
for x in [-.44,.44]:box('Cabinet foot',(x,-.03,0),(.07,.06,.42),'Metal',.006)
export('BenchLowerCabinet')
# Grip at top, two real wire loops centred around the selected key below.
box('Puller grip',(0,.22,0),(.045,.19,.055),'Terracotta',.014)
for x in [-.052,.052]:tube('Spring wire',[ (x*.25,.16,0),(x,.11,-.045),(x,0,-.065),(x,-.018,0),(x,0,.065),(x,.11,.045),(x*.25,.16,0)],.0025,'Steel')
export('WireKeycapPuller')

