# PildiÜlesanne TreeView Versioon

## Kirjeldus
See on C# Windows Forms rakendus graafilise liidesega, mis sisaldab kolme põhimoodulit:

1. **Pildiülesanne (Pildivaatur)**  
   - Kuvab pilte (`.jpg`, `.png`, `.bmp`).  
   - Nupud: näita, kustuta, muuda taustavärvi, sulge rakendus.  
   - Lisafunktsioonid:  
     - Pildi venitus sisse/ välja lülitamine.  
     - Pildi pööramine horisontaalselt või vertikaalselt.  
     - Värvide invertimine.  
     - Piltide slaidiseanss valitud kaustas.  
     - Piltide salvestamine PNG, JPEG või BMP formaadis.

2. **Matemaatiline Quiz**  
   - Neli tüüpi ülesandeid: liitmine, lahutamine, korrutamine, jagamine.  
   - Raskusastmed: `Lihtne`, `Keskmine`, `Raske`.  
   - Harjutusrežiim ilma ajapiiranguta.  
   - Taimer 30-sekundilise piiriga (tavaline režiim).  
   - Edusammude riba ja õigeid vastuseid näitav silt.  
   - Vihjenupp, mis näitab ühte vale vastust.  
   - Näited genereeritakse juhuslikult vastavalt valitud raskusastmele.

3. **Matching Game**  
   - 4×4 mängulaud kaartidega.  
   - Eesmärk: leida paarid identsete sümbolitega.  
   - 60-sekundiline taimer, järelejäänud aeg kuvatakse.  
   - Vali kaardile värv (`Red`, `Green`, `Blue`, `Yellow`).  
   - Vali mängulaua teema (`Sinine`, `Roheline`, `Punane`).  
   - Nupp uue mängu alustamiseks.  
   - Võidu tuvastamine ja teade võidu korral.

## Navigeerimine
Vasakul TreeView menüü abil saab vahetada moodulite vahel:
- `Pildiülesanne`
- `Matemaatiline Quiz`
- `Matching Game`

## Nõuded
- .NET Framework, mis toetab Windows Forms.  
- Windows operatsioonisüsteem graafilise liidesega.

## Käivitamine
1. Ava projekt Visual Studios.  
2. Koosta projekt (`Build Solution`).  
3. Käivita rakendus (`Start Debugging` või `Ctrl+F5`).  
4. Kasuta vasakpoolset menüüd (TreeView) moodulite vahetamiseks.

## Märkused
- Kõik funktsioonid on implementeeritud ühes klassis `Form1`.  
- Taimerid on kasutusel quiz’i ja matching game jaoks.  
- Nuppude, ComboBox’i ja CheckBox’i sündmuste käsitlejad on koodis seadistatud.
