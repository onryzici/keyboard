import urllib.request,json,pathlib,subprocess,zipfile,io
from PIL import Image,ImageDraw,ImageFont
root=pathlib.Path('/Users/trexoinnovation/Desktop/keyboard/LittleSwitch/Assets/LittleSwitch')
a=root/'Resources'/'SwitchAudio';a.mkdir(parents=True,exist_ok=True)
base='https://raw.githubusercontent.com/hainguyents13/mechvibes/main/'
credit=root/'ThirdParty'/'MechVibes';credit.mkdir(exist_ok=True)
(credit/'LICENSE.txt').write_bytes(urllib.request.urlopen(base+'LICENSE').read())
for fam,col in [('Linear','red'),('Tactile','brown'),('Clicky','blue')]:
 pack='cherrymx-'+col+'-pbt';p='src/audio/'+pack+'/'
 cfg=urllib.request.urlopen(base+p+'config.json').read();(credit/(pack+'.json')).write_bytes(cfg)
 ogg=pathlib.Path('/tmp/'+pack+'.ogg');ogg.write_bytes(urllib.request.urlopen(base+p+'sound.ogg').read())
 defs=json.loads(cfg)['defines'];out=a/fam;out.mkdir(exist_ok=True)
 for i,key in enumerate(['16','17','18','19','20','30','31','32']):
  start,duration=defs[key]
  subprocess.run(['ffmpeg','-v','error','-y','-ss',str(start/1000),'-i',str(ogg),'-t',str(duration/1000),'-ac','1','-ar','44100','-af',f'afade=t=out:st={max(0,duration/1000-.008)}:d=0.008','-c:a','pcm_s16le',str(out/f'{fam}-{i+1:02}.wav')],check=True)
 print('Prepared',fam,flush=True)
# Original vector-like mat artwork; reference proportions, no copied branding.
out=root/'Art';out.mkdir(exist_ok=True)
w,h=2048,1060;im=Image.new('RGB',(w,h),'#28617e');d=ImageDraw.Draw(im)
font=ImageFont.truetype('/System/Library/Fonts/Helvetica.ttc',20);small=ImageFont.truetype('/System/Library/Fonts/Helvetica.ttc',15)
x0,y0,x1,y1=76,60,1972,992; step=(x1-x0)/40
for i in range(81):
 x=x0+i*step/2;d.line((x,y0,x,y1),fill='#7da2ac' if i%2==0 else '#42758b',width=2 if i%10==0 else 1)
for j in range(40):
 y=y1-j*step/2
 if y<y0:break
 d.line((x0,y,x1,y),fill='#7da2ac' if j%2==0 else '#42758b',width=2 if j%10==0 else 1)
d.rectangle((x0,y0,x1,y1),outline='#adccca',width=3)
for i in range(41):
 x=x0+i*step;d.text((x-7,y1+21),str(i),font=small,fill='#bad4ce');d.line((x,y1+3,x,y1+15),fill='#bad4ce',width=2)
 if i%5==0:d.text((x-10,23),str(i),font=font,fill='#bad4ce')
for j in range(20):
 y=y1-j*step
 d.text((37,y-8),str(j),font=small,fill='#bad4ce');d.line((x0-13,y,x0-3,y),fill='#bad4ce',width=2)
import math
for angle in [30,45,60]:
 ex=min(x1,x0+(y1-y0)/math.tan(math.radians(angle)));ey=y1-(ex-x0)*math.tan(math.radians(angle))
 d.line((x0,y1,ex,ey),fill='#99b8bc',width=2)
 x=x0+(ex-x0)*.8;y=y1+(ey-y1)*.8;d.rectangle((x-20,y-12,x+22,y+12),fill='#28617e');d.text((x-16,y-9),str(angle)+'°',font=small,fill='#d0ddd3')
d.rounded_rectangle((1595,878,1915,957),radius=8,fill='#28617e',outline='#8cafb5',width=2);d.text((1621,900),'CUTTING MAT   /   A2',font=font,fill='#bed5d0')
im.save(out/'CuttingMat.png')
z=urllib.request.urlopen('https://opengameart.org/sites/default/files/old_computer_blend_0.zip').read();zipfile.ZipFile(io.BytesIO(z)).extractall('/tmp/retro-computer');print('Computer source extracted')
