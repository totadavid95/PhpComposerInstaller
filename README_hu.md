# Automatikus PHP, Xdebug, Composer telepítő

**A legfrissebb kiadás az alábbi gombra kattintva tölthető le:**

[![Letöltés](https://img.shields.io/badge/Telep%C3%ADt%C5%91%20let%C3%B6lt%C3%A9se-32A852?style=for-the-badge&logoColor=white&logo=DocuSign)](https://github.com/totadavid95/PhpComposerInstaller/releases/latest/download/PhpComposerInstaller.zip)

[Click here for ENGLISH documentation. (Kattints ide az ANGOL nyelvű dokumentációért!)](https://github.com/totadavid95/PhpComposerInstaller/blob/master/README.md)

- Letöltés után **csomagold ki** a *.zip* fájlt, majd futtasd a kicsomagolt `PhpComposerInstall.exe`-t. **NE FUTTASD KÖZVETLENÜL AZ ARCHÍVUMBÓL!**
- A telepítő futtatásához legalább [.NET 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) szükséges. (Windows 10 és 11 rendszereken ez a követelmény alapból teljesül.)

Jelen dokumentáció mindig a legfrissebb kiadásra vonatkozik, a régebbi verziók a GitHub history-ban tekinthetők meg.

Tartalom:
- [Automatikus PHP, Xdebug, Composer telepítő](#automatikus-php-xdebug-composer-telepítő)
  - [A projekt leírása](#a-projekt-leírása)
  - [Parancssori kapcsolók](#parancssori-kapcsolók)
  - [Gyakran előforduló problémák](#gyakran-előforduló-problémák)
    - [Hibák jelentése](#hibák-jelentése)
  - [Kézi telepítés menete (Windows)](#kézi-telepítés-menete-windows)
    - [PHP telepítése](#php-telepítése)
    - [Visual C++ Redistributable telepítése](#visual-c-redistributable-telepítése)
    - [Xdebug kiegészítő telepítése a PHP-hoz](#xdebug-kiegészítő-telepítése-a-php-hoz)
    - [Composer telepítése](#composer-telepítése)
    - [Telepítések hozzáadása a Path környezeti változóhoz](#telepítések-hozzáadása-a-path-környezeti-változóhoz)
  - [Telepített eszközök tesztelése](#telepített-eszközök-tesztelése)
  - [Kézi telepítés menete (Linux)](#kézi-telepítés-menete-linux)
  - [Segítség és támogatás](#segítség-és-támogatás)

## A projekt leírása
Ezt a telepítőt elsősorban az *Eötvös Loránd Tudományegyetemen* általam is oktatott *Szerveroldali webprogramozás* tárgy hallgatóinak készítettem segítségképpen, hogy a PHP, Xdebug és Composer egyszerűen telepíthető és beállítható legyen Windows környezetben.

A telepítő **nem igényel rendszergazdai** jogokat, mivel csak az aktuális Windows felhasználó szintjén működik, ezért a laborszámítógépeken is használható. Ez megkímél minket sok elvesztegetett időtől és felesleges fejfájástól.

A telepítő a futtatásakor letölti az említett programok legfrissebb változatait, és telepíti, majd beállítja őket. Ha végeztél már korábban telepítést, akkor először a meglévő telepítés törlődik, és a legfrissebb verziókkal egy tiszta telepítés játszódik le. Ez használható hibás telepítés javítására vagy a programok frissítésére is.

## Parancssori kapcsolók
A telepítő opcionálisan az alábbi parancssori kapcsolókkal is meghívható, például `PhpComposerInstall.exe --no-cleanup`.

Elérhető kapcsolók:

- `--uninstall` / `-ui`: teljesen eltávolítja a telepítést, amit korábban ez a telepítő telepített
- `--no-cleanup` / `-nc`: ne távolítása el a letöltött / ideiglenes fájlokat a telepítés után
- `--xdebug` / `-xd`: telepíti és beállítja az Xdebug-ot
- `--no-vc-redist` / `--nvc`: ne telepítse a VC Redistributable-t

## Gyakran előforduló problémák
Ha a telepítő vagy a telepített szoftverek valamelyike nem működik megfelelően, akkor elképzelhető, hogy ezen okok valamelyike a felelős érte:
- Egyes víruskereső programok valamelyik telepítési lépést (jellemzően a Composert) tévesen kártevőnek minősítik (másnéven false positive riasztás). Ha ilyen szoftvert használsz, próbáld meg szüneteltetni a védelmét, és úgy futtatni a telepítőt. A telepítő nem "vírusos", a forráskód megtalálható itt GitHub-on.
- Elképzelhető, hogy valamilyen "ingadozás" történt a hálózatban és megszakadt a letöltés. Érdemes még egy próbát tenni, akár picit később.
- Hibákhoz vezethet, tipikusan pl. a Tinker esetében, ha a Windows **felhasználónévben** ékezet van. Ennek egy lehetséges megoldására [készítettem egy videót](https://www.youtube.com/watch?v=_zL3Pxnidj0).

### Hibák jelentése
Amennyiben a fentiek elolvasása után a probléma továbbra is fennáll, lásd a [Segítség és támogatás](#segítség-és-támogatás) alcímet.

## Kézi telepítés menete (Windows)
A telepítő lényegében az alábbi lépéseket hajtja végre automatizáltan.

### PHP telepítése
- A PHP hivatalos oldalán a jelenleg támogatott PHP verziók szerepelnek: [https://windows.php.net/download](https://windows.php.net/download) 
- Töltsd le a neked leginkább megfelelő (lehetőleg a legrossebb) __x86 / x64 Non Thread Safe__ verziót. Az x86 azt jelenti, hogy 32 bites rendszerre való, az x64 pedig azt, hogy 64 bites rendszerre.
- Csomagold ki a letöltött *.zip* archívumot.
- A PHP-nek szüksége van egy __php.ini__ névű konfigurációs fájlra, amely nem szerepel a kicsomagolt archívumban. Készíts másolatot a __php.ini-development__ fájlról (amely lényegében egy mintakonfiguráció), majd nevezd át __php.ini__-re.
- Nyisd meg a __php.ini__ fájlt, és hajtsd végre a következő módosításokat:
  - Keress rá a következő sorra:  `;extension_dir = "ext"`, és távolítsd el a `;`-t (komment jele) az elejéről. Erre azért van szükség, hogy a PHP a saját mappájában keresse majd a kiegészítőket. (Mivel lokálisan telepítettük, és alapból nem jó helyen keresné).
  - Ezután engedélyezned kell néhány kiegészítőt, amikre azért lesz szükség, hogy a PHP által későbbiekben futtatott Composer, Laravel, stb megfelelően tudjon működni. Vagyis az alábbi sorokból is távolísd el a kommentjelet, engedélyezve ezzel az adott kiegészítőket:
    ```ini
    ;extension=curl
    ;extension=fileinfo
    ;extension=mbstring
    ;extension=openssl
    ;extension=pdo_mysql
    ;extension=pdo_sqlite
    ```
- Ha kész vagy, mentsd el a __php.ini__ fájlt.
- Az egész PHP mappa tartalmát másold át ide: 
  `%LOCALAPPDATA%\Programs\php` (ha még nincs a `Programs` mappában `php` mappa, létre kell hozni).

Tipp: a fájlkezelő címsorába másoljuk be, hogy `%LOCALAPPDATA%\Programs`!

### Visual C++ Redistributable telepítése

A [telepítési követelmények](https://www.php.net/manual/en/install.windows.requirements.php) leírják, hogy Windowson szükséges a Visual C++ Redistributable jelenléte a PHP futtatásához. Ezért, ha a PHP az első futtatáskor "dll hibát" ír, telepítsd az OS architektúrádnak (32 vagy 64 bit) megfelelő Visual C++ runtime library-t:
- [vc_redist.x86.exe](https://aka.ms/vs/16/release/vc_redist.x86.exe) (32 bit)
- [vc_redist.x64.exe](https://aka.ms/vs/16/release/vc_redist.x64.exe) (64 bit)

### Xdebug kiegészítő telepítése a PHP-hoz
- A [https://xdebug.org/download](https://xdebug.org/download) oldalról a __Windows binaries__ részből válaszd ki a telepített PHP verziónak megfelelőt!
- A Xdebug-ot töltsd le __php_xdebug.dll__ néven!
- Másold be a PHP __ext__ mappájába az imént letöltött __php_xdebug.dll__ fájlt!
- Nyisd meg a __php.ini__ fájlt! Az ini fájl az alábbi mintát követi:
  ```ini
  [szekcio_neve]
  beallitas_neve = beallitas_erteke
  ```
  - Először engedélyezd az Xdebug-ot Zend extension-ként a `[PHP]` szekció alatt:
    ```ini
    [PHP]
    ; Ennek a kommentnek a helyén elég sok beállítást és kommentet fogsz találni, 
    ; de a következő [szekció] elé, még a [PHP] szekció aljára írd be, hogy:
    zend_extension = xdebug
    ``` 
  - Utána pedig add meg az Xdebug-hoz tartozó beállításokat a __php.ini__ fájl legalján, egy új `[xdebug]` szekció felvételével:
    ```ini
    [xdebug]
    xdebug.cli_color = 1
    xdebug.client_host = localhost
    xdebug.client_port = 9003
    ```
    - Az összes Xdebug-hoz tartozó beállítást részletes leírással együtt a [hivatalos dokumentációban találod meg](https://xdebug.org/docs/all_settings).
- Mentsd el a __php.ini__ fájlt!
- Ha eközben futott olyan PHP-s folyamat a gépen, amely az imént konfigurált PHP-hoz köthető, akkor azt a folyamatot újra kell indítani az új PHP konfiguráció érvényesüléséhez.

### Composer telepítése
- A [https://getcomposer.org/download/latest-2.x/composer.phar](https://getcomposer.org/download/latest-2.x/composer.phar) linkre kattintva töltsd le a legfrissebb Composer v2 kiadást.
- A letöltött `composer.phar` nevű fájlt másold a következő mappába: `%LOCALAPPDATA%\Programs\composer` (ha még nincs a `Programs` mappában `composer` mappa, létre kell hozni).
- Mivel ez egy PHP fájl (PHAR - **PH**p **AR**chive), ezért a Windows nem tudja direktben futtatni (mint a php.exe-t), hanem kell hozzá egy olyan parancsfájl, amit az OS meg tud hívni, és ami átadja a PHP futtató környezetnek a PHAR fájlt. Így válik majd lehetővé, hogy a CMD ablakban műkdödjön a `composer` parancs, tehát lényegében ezt a `bat` fájlt fogja hívni. A lényeg: ebben a __composer__ mappában csinálj egy `composer.bat` nevű fájlt is, aminek a tartalma a következő egy sor legyen:
 
  ```bat
  @php "%~dp0composer.phar" %*
  ```
- Ezáltal lényegében a `composer.bat` meghívásakor a PHP értelmező futtatja a Composert, ami egy PHP fájl, átadva neki a paramétereket is (% a végén).

### Telepítések hozzáadása a Path környezeti változóhoz
Lényegében ezen a ponton majdnem kész vagy a telepítéssel, azonban jelenleg csak úgy hívható a PHP és a Composer, ha megadod az abszolút útvonalat hozzá.

Ahhoz, hogy bármilyen megnyitott parancssorból egyszerűen meg lehessen hívni a `php`, illetve a `composer` parancsokat, hozzá kell adni a `%localappdata/Programs` mappán belül imént elkészített **két mappát** a Path környezeti változóhoz.

Ez a legegyszerűbben úgy tehető meg, hogy beírod a Start menü keresőbe, hogy "Fiókhoz tartozó környezeti változók szerkesztése", vagy angol Windowson "Edit environment variables for your account". Alternatív lehetőség a Win+R, majd `rundll32 sysdm.cpl,EditEnvironmentVariables` parancs kiadása.

Itt fontos, hogy a megjelenő ablak fenti részében dolgozz, mivel az vonatkozik a __felhasználó__ környezeti változóira, és a labor gépeken amúgy sem lehet módosítani a rendszerhez tartozó tulajdonságokat.

**A változás érvénybe léptetéséhez az esetlegesen korábbról nyitva lévő parancssorokat be kell zárni.**

## Telepített eszközök tesztelése
A telepítés sikerességét a `php -v` és `composer -V` parancsok futtatásával ellenőrizheted. Mindkét esetben meg kell jelenjenek a telepített verzió adatai, továbbá a PHP-nál az Xdebug-ra vonatkozó információ is.

A PHP-t részletesebben úgy tudod tesztelni, ha létrehozol pl. egy *test.php* nevű fájlt, amibe beleírod ezt:
```php
<?php
    // PHP adatainak kiírása
    phpinfo();

    // Xdebug adatainak kiírása
    xdebug_info();
?>
```

Ezt követően nyiss egy parancssort az adott `test.php`-t tartalmazó mappában (vagy `cd` segítségével navigálj oda), és add ki a következő parancsot: 
`php -S localhost:3000 test.php`

Ekkor a böngészőben megtekintheted a [http://localhost:3000/](http://localhost:3000/) oldalt. Ez egy átfogó riportot fog megjeleníteni a PHP környezetről (beleértve a verziószámot és az engedélyezett kiegészítőket). A parancssort zárd be a végén! A parancssor bezárásakor természetesen a kiszolgáló is leáll.

Ugyanez a két teszt elvégezhető az `InstallTest` telepítő mappájában lévő eszközökkel is.

## Kézi telepítés menete (Linux)
Az automatikus telepítő csak Windowson működik, azonban ez a fejezet segíthet a Linux alatti telepítésben. Az folyamat [ezen a leíráson](https://tecadmin.net/how-to-install-php-8-on-ubuntu-20-04/) alapul, valamint Xubuntu 20.04 rendszeren teszteltük.

1. Csomaglista frissítése:
   
   ```shell
   sudo apt update
   ```
2. Telepíteni kell egy olyan csomagot, ami segít a PPA-k kezelésében:
   
   ```shell
   sudo apt install software-properties-common -y 
   ```
3. Hozzá kell adni egy új [PPA csomagrepository-t](https://launchpad.net/~ondrej/+archive/ubuntu/php), ami a PHP-t kezeli:
   
   ```shell
   sudo add-apt-repository ppa:ondrej/php 
   ```
4. Mostmár a rendszer látja a PHP-s csomagokat, tehát az alap PHP telepítése következik. Meg kell adni a verziószámot, de értelemszerűen el is lehet térni az itt példaként mutatott verziószámtól (a leírás megírásának pillanatában a 8.1 a legújabb), csak következetesen kell használni, és mindenhol ugyanazt kell utána megadni.
  
   ```shell
   sudo apt install php8.1
   ```
5. Szükséges PHP kiegészítők telepítése. Ezek alapvetően az alábbiak, de igény szerint telepíthetsz továbbiakat is:
   
   ```shell
   sudo apt install php8.1-cli php8.1-xml php8.1-curl php8.1-fileinfo php8.1-mbstring php8.1-sqlite3 php8.1-xdebug
   ```
6. Teszteld le, hogy működik-e a PHP! Add ki az alábbi parancsot:
   
   ```shell
   php -v
   ```
   - Ennek eredményeként pedig az alábbihoz hasonló kimenetet kell látnod (értelemszerűen nem pont ezekkel a verziószámokkal):
     ```text
     PHP 8.1.2 (cli) (built: Jan 24 2022 10:42:33) (NTS)
     Copyright (c) The PHP Group
     Zend Engine v4.1.2, Copyright (c) Zend Technologies
         with Zend OPcache v8.1.2, Copyright (c), by Zend Technologies
         with Xdebug v3.1.2, Copyright (c) 2002-2021, by Derick Rethans
     ```

7. Composer telepítése. Ezen a ponton már elérhető a PHP CLI, tehát a [Composer honlapján](https://getcomposer.org/doc/faqs/how-to-install-composer-programmatically.md) megadott letöltést, telepítést segítő szkript is működik:

    ```shell
    #!/bin/sh

    EXPECTED_CHECKSUM="$(php -r 'copy("https://composer.github.io/installer.sig", "php://stdout");')"
    php -r "copy('https://getcomposer.org/installer', 'composer-setup.php');"
    ACTUAL_CHECKSUM="$(php -r "echo hash_file('sha384', 'composer-setup.php');")"

    if [ "$EXPECTED_CHECKSUM" != "$ACTUAL_CHECKSUM" ]
    then
      >&2 echo 'ERROR: Invalid installer checksum'
      rm composer-setup.php
      exit 1
    fi

    php composer-setup.php --quiet
    RESULT=$?
    rm composer-setup.php
    exit $RESULT
    ```
    - Ezt a szkriptet mentsd el (pl. `composer.sh` néven), adj neki futtatási jogot, majd hívd meg.
8. Az előző pont hatására létrejött egy `composer.phar` nevű fájl. Azonban ez még nem érhető el mindenhonnan. Ahhoz, hogy a Composer globálisan is elérhető legyen a gépeden, ezt az imént létrejött fájlt be kell másolni a `bin` mappába:
   
   ```shell
   sudo mv composer.phar /usr/local/bin/composer
   ```
9. Composer tesztelése. Győződj meg róla, hogy a Composer megfelelően működik-e mindenhol, az alábbi parancs kiadásával:

   ```shell
   composer -V
   ```
## Segítség és támogatás
Ha a telepítéssel kapcsolatban kérdésed van, keress bátran és segítek szívesen (totadavid95@inf.elte.hu vagy Teams chat). Ha van GitHub account-od, issue-t is nyithatsz.
