# PHP, Xdebug, Composer telepítő
Ezt a telepítőt elsősorban az általam is oktatott Szerveroldali webprogramozás tárgy hallgatóinak készítettem segítségképpen, hogy a PHP, Xdebug és Composer egyszerűen telepíthető és beállítható legyen Windowsos környezetben. 

## Működés
A telepítő a futtatásakor letölti az említett programok legfrissebb változatait, és telepíti, beállítja őket. Ha a programmal végeztünk már korábban telepítést, majd pedig újra futtatjuk, akkor törli az aktuális telepítést, és a legfrissebb verziókkal újra végez egy tiszta telepítést. Ez használható hibás telepítés javítására vagy a programok frissítésére is. A telepítő nem igényel rendszergazdai jogokat, mivel csak az aktuális Windows felhasználó szintjén működik.

## Motiváció
Ahhoz, hogy az órákon megfelelően tudjunk dolgozni, valamint gyakorolni, az egyetemi labor gépeken (illetve sok esetben az otthoni gépeken is) be kell állítani egy lokális környezetet. Ennek a környezetnek a beállítása, hogy kellően egyszerű legyen, és mégis jól működjön, még a laborgépeken is (amiken alapból nincs PHP, sem Composer), nem feltétlenül magától értetődő feladat. Ennek érdekében dolgoztuk ki ezt a telepítési rendszert, amit ha egy felhasználó egyszer elvégez, onnantól mindig tudja használni azt az adott gépen.

## Hibát találtál?
Ha a telepítő vagy a telepített szoftverek valamelyike nem működik megfelelően, akkor elképzelhető, hogy ezen okok valamelyike a felelős érte:
- Egyes víruskereső programok (tipikusan pl. az Avast) valamelyik telepítési lépést, jellemzően a Composert tévesen kártevőnek minősíti (másnéven false positive riasztás). Ha ilyen szoftvert használsz, próbáld meg szüneteltetni a védelmét, és úgy futtatni a telepítőt. A telepítő nem "vírusos", a forráskód megtalálható itt GitHub-on, a build is itt történik, Actions segítségével. A folyamat teljesen átlátható és biztonságos.
- Elképzelhető, hogy valamilyen "ingadozás" történt a hálózatban és a telepítő valamiért nem tudott valamelyik kiszolgálóhoz kapcsolódni, ahonnan letölti a programokat vagy időközben megszakadt a kapcsolat, pl. letöltéskor. Érdemes még egy próbát tenni, akár picit később.

Ha ezek egyike sem vezet megoldásra, akkor kérlek, hogy nyiss egy GitHub issue-t, vagy írj nekem Teamsen egy üzenetet, amelyben részletezed a problémát, és az oda vezető lépések sorozatát. Igyekszek minél előbb megoldást találni a problémára.

## Kézi telepítés
Arra az esetre, ha a telepítő valamiért nem működne, alább ismertetem a telepítés és a beállítás menetét (amit jobb esetben a program hajt végre helyettünk).

### PHP telepítése
- A [https://windows.php.net/download](https://windows.php.net/download) oldalról a legfrissebb PHP kiadás __x64 Non Thread Safe__ verzióját kell letölteni, majd kicsomagolni.
- A __php.ini-development__ fájlról készítsünk egy másolatot, amit nevezzünk el __php.ini__-nek.
- Nyissuk meg a __php.ini__ fájlt, és hajtsuk végre a következő módosításokat:
  - Keressünk rá a következő sorra:  `;extension_dir = "ext"`, és "kommentezzük ki", vagyis hagyjuk el a pontosvesszőt az elejéről. Erre azért van szükség, hogy a PHP a saját mappájában keresse majd a kiegészítőket (mivel lokálisan telepítjük, és alapból nem jó helyen keresné).
  - Most pedig engedélyeznünk kell néhány kiegészítőt. A következő sorokat is élesítenünk kell (ezek kb. egy blokkban vannak):
     ```ini
    ;extension=fileinfo
    ;extension=mbstring
    ;extension=openssl
    ;extension=pdo_mysql
    ;extension=pdo_sqlite
    ```
- Ha kész vagyunk, mentsük el a __php.ini__ fájlt.
- Az egész mappa tartalmát másoljuk át ide: 
  `%LOCALAPPDATA%\Programs\php` (ha még nincs a Programs mappában php mappa, létre kell hozni).

Tipp: Fájlkezelő címsorába másoljuk be, hogy `%LOCALAPPDATA%\Programs`

#### XDebug kiegészítő telepítése a PHP-hoz
- A [https://xdebug.org/download](https://xdebug.org/download) oldalról a __Windows binaries__ részből válasszuk ki a telepített PHP verziónak megfelelőt (ne felejtsük el, hogy x86 / x64 kell-e, a TS a Thread Safe-t jelenti a végén, az aktuális PHP verziónkat és az architektúrát pedig a `php -v` parancs adja meg).
- Ha sikerült, mentsük el __php_xdebug.dll__ néven.
- Másoljuk be a PHP __ext__ mappájába a __php_xdebug.dll__ fájlt.
- Nyissuk meg a __php.ini__ fájlt, és írjuk az alábbiakat a fájl legaljára:
  ```ini
  [XDebug]
  xdebug.remote_enable = 1
  xdebug.remote_autostart = 1
  zend_extension=xdebug
  ```
- Mentsük el a __php.ini__ fájlt.
- Ha fut valamilyen PHP-s folyamat a gépen, akkor azt újra kell indítani annak érdekében, hogy betöltse ezt a kiegészítőt is.

#### Composer telepítése
- A [https://getcomposer.org/composer-stable.phar](https://getcomposer.org/composer-stable.phar) linkre kattintva töltsük le a legfrissebb Composer kiadást.
- A letöltött fájlt `composer.phar` néven bemásoljuk a következő mappába: `%LOCALAPPDATA%\Programs\composer` (ha még nincs a Programs mappában composer mappa, létre kell hozni).
- Szintén ebben a __composer__ mappában csinálunk egy `composer.bat` fájlt, aminek a tartalma a következő egy sor legyen: 
 `@php "%~dp0composer.phar" %*`

#### Hozzáadás a Path környezeti változóhoz
Ahhoz, hogy parancssorból meg tudjuk hívni a php, illetve a composer parancsokat, hozzá kell az előbb elkészített két mappát adni a Path környezeti változóhoz.

Ez a legegyszerűbben úgy tehető meg, hogy beírjuk a Start menü keresőbe, hogy "Fiókhoz tartozó környezeti változók szerkesztése", vagy angol Windowson "Edit environment variables for your account". Alternatív lehetőség a Win+R, majd `rundll32 sysdm.cpl,EditEnvironmentVariables` parancs kiadása.

Itt fontos, hogy a megjelenő ablak fenti részében dolgozzunk, mivel az vonatkozik a __felhasználó__ környezeti változóira, és a labor gépeken nem lehet módosítani a rendszerhez tartozó tulajdonságokat.

## Telepítés tesztelése
Ha minden fent ismertetett műveletet sikeresen elvégeztünk, a parancssorban elérhetővé válik a php és a composer (a már megnyitott parancssorokban nem, azokat újra meg kell nyitni), ezt így próbálhatjuk ki:
`php -v` és `composer -V`
Mindkét esetben ki kell írja a telepített verzió adatait.

A PHP-t részletesebben úgy tudod tesztelni, ha csinálsz például egy *test.php* nevű fájlt, amibe beleírod ezt:
```php
<?php
	phpinfo();
?>
```

Ezt követően nyiss egy parancssort az adott mappában, vagy cd parancssal lépj bele, utána add ki a következő parancsot: 
`php -S localhost:3000 test.php`

Majd a böngészőben nyisd meg a [http://localhost:3000/](http://localhost:3000/) oldalt.

Ez egy átfogó jelentést ad a PHP adatairól és beállításairól, pl. láthatod a verziószámát, bekapcsolt kiegészítőket, van-e XDebug, stb. A parancssort zárd be a végén.

## Segítség, támogatás
Ha a telepítéssel kapcsolatban kérdésed van, keress bátran és segítek szívesen (totadavid95@inf.elte.hu vagy Teams chat).

## Fejlesztési lehetőségek
Nyilván vannak még fejlesztési lehetőségek, mint pl. a telepített szoftverek verziójának kiválasztása (jelenleg csak a legfrissebbet tölti le), parancssori alkalmazás helyett valami szebb, GUI-s változat, viszont ezeket akkor csak akkor csinálom meg, ha lesz elegendő időm rá.
