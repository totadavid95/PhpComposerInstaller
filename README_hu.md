# Automatikus PHP, Xdebug, Composer telepítő

**A legfrissebb kiadás az alábbi gombra kattintva tölthető le:**

[![Letöltés](https://img.shields.io/badge/Download%20Installer-32A852?style=for-the-badge&logoColor=white&logo=DocuSign)](https://github.com/totadavid95/PhpComposerInstaller/releases/latest/download/PhpComposerInstaller.zip)

[Click here for ENGLISH documentation. (Kattints ide az ANGOL nyelvű dokumentációért!)](https://github.com/totadavid95/PhpComposerInstaller/blob/master/README.md)

- Letöltés után **csomagold ki** a *.zip* fájlt, majd futtasd a kicsomagolt `PhpComposerInstall.exe`-t. **NE FUTTASD KÖZVETLENÜL AZ ARCHÍVUMBÓL!**
- A telepítő futtatásához legalább [.NET 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472) szükséges. (Windows 10 és 11 rendszereken ez a követelmény alapból teljesül.)

Jelen dokumentáció mindig a legfrissebb kiadásra vonatkozik, a régebbi verziók a GitHub history-ban tekinthetők meg.

Tartalom:
- [Automatikus PHP, Xdebug, Composer telepítő](#automatikus-php-xdebug-composer-telepítő)
  - [A projekt ismertetése](#a-projekt-ismertetése)
  - [Korábbi verziók letöltése](#korábbi-verziók-letöltése)
  - [A telepítő működése](#a-telepítő-működése)
  - [Parancssori kapcsolók](#parancssori-kapcsolók)
  - [Motiváció](#motiváció)
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

## A projekt ismertetése
Ezt a telepítőt elsősorban az *Eötvös Loránd Tudományegyetemen* általam is oktatott *Szerveroldali webprogramozás* tárgy hallgatóinak készítettem segítségképpen, hogy a PHP, Xdebug és Composer egyszerűen telepíthető és beállítható legyen Windowsos környezetben.

## Korábbi verziók letöltése
Az összes eddig kiadott telepítő elérhető a [Releases](https://github.com/totadavid95/PhpComposerInstaller/releases) oldalon. Az adott verzióhoz tartozó *PhpComposerInstaller.zip* fájlban van a futtatható telepítő, a másik kettő csak a forráskód zippelve.

## A telepítő működése
A telepítő a futtatásakor letölti az említett programok legfrissebb változatait, és telepíti, beállítja őket.

Ha a programmal végeztél már korábban telepítést, majd pedig újra futtatod, akkor törli az aktuális telepítést, és a legfrissebb verziókkal újra végez egy tiszta telepítést. Ez használható hibás telepítés javítására vagy a programok frissítésére is.

A telepítő nem igényel rendszergazdai jogokat, mivel csak az aktuális Windows felhasználó szintjén működik.

## Parancssori kapcsolók
A telepítő opcionálisan az alábbi parancssori kapcsolókkal is meghívható, például `PhpComposerInstall.exe --no-cleanup`.

Elérhető kapcsolók:
- `--uninstall`
  - Telepítés teljes törlése
- `--no-cleanup`
  - Telepítés után hagyja meg a letöltött / ideiglenes fájlokat

## Motiváció
Ahhoz, hogy az órákon megfelelően tudjunk dolgozni, valamint gyakorolni, az egyetemi laborgépeken (illetve sok esetben az hallgatók otthoni gépein is) be kell állítani egy lokális fejlesztőkörnyezetet. Ennek a környezetnek a beállítása úgy, hogy egyszerű legyen és mégis megfelelően működjön még a laborgépeken is (amiken alapból nincs sem PHP, sem Composer), nem feltétlenül magától értetődő feladat - de amennyiben az lenne, még akkor is értékes időt venne el a gyakorlatokból feleslegesen. Ennek érdekében dolgoztuk ki ezt a telepítési rendszert, amit ha egy felhasználó egyszer elvégez, onnantól minden órán tudja használni a szükséges eszközöket az adott gépen.

## Gyakran előforduló problémák
Ha a telepítő vagy a telepített szoftverek valamelyike nem működik megfelelően, akkor elképzelhető, hogy ezen okok valamelyike a felelős érte:
- Egyes víruskereső programok (tipikusan pl. az Avast) valamelyik telepítési lépést, jellemzően a Composert tévesen kártevőnek minősíti (másnéven false positive riasztás). Ha ilyen szoftvert használsz, próbáld meg szüneteltetni a védelmét, és úgy futtatni a telepítőt. A telepítő nem "vírusos", a forráskód megtalálható itt GitHub-on, a build is itt történik, Actions segítségével. A folyamat teljesen átlátható és biztonságos.
- Elképzelhető, hogy valamilyen "ingadozás" történt a hálózatban és a telepítő valamiért nem tudott valamelyik kiszolgálóhoz kapcsolódni, ahonnan letölti a programokat vagy időközben megszakadt a kapcsolat, pl. letöltéskor. Érdemes még egy próbát tenni, akár picit később.
- Hibákhoz vezethet, tipikusan pl. a Tinker esetében, ha a Windows **felhasználónévben** ékezet van, hiszen ennélfogva minden útvonal ékezetes, ami az adott felhasználóhoz tartozik. Mivel a telepítő a felhasználó szintjén végzi a telepítést, ezért a telepített PHP és Composer szintén érintett. Ennek egy lehetséges megoldására [készítettem egy videót](https://www.youtube.com/watch?v=_zL3Pxnidj0).

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
- A [https://xdebug.org/download](https://xdebug.org/download) oldalról a __Windows binaries__ részből válaszd ki a telepített PHP verziónak megfelelőt (tartsd szem előtt, hogy x86 (32 bit) / x64 (64 bit) kell-e, a TS / NTS a Thread Safe-t / Non-Thread Safe-t jelenti a végén, az aktuális PHP verziódat és az architektúrát pedig a `php -v` parancs adja meg).
- A Xdebug-ot töltsd le __php_xdebug.dll__ néven.
- Másold be a PHP __ext__ mappájába az imént letöltött __php_xdebug.dll__ fájlt.
- Nyisd meg a __php.ini__ fájlt. Az ini fájl az alábbi mintát követi:
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
- Mentsd el a __php.ini__ fájlt.
- Ha ezközben futott olyan PHP-s folyamat a gépen, amely az imént konfigurált PHP-hoz köthető, akkor azt a folyamatot újra kell indítani annak érdekében, hogy betöltse az új PHP beállításokat.

### Composer telepítése
- A [https://getcomposer.org/download/latest-2.x/composer.phar](https://getcomposer.org/download/latest-2.x/composer.phar) linkre kattintva töltsd le a legfrissebb Composer v2 kiadást.
- A letöltött `composer.phar` nevű fájlt másold a következő mappába: `%LOCALAPPDATA%\Programs\composer` (ha még nincs a `Programs` mappában `composer` mappa, létre kell hozni).
- Mivel ez egy PHP fájl (PHAR - **PH**p **AR**chive), ezért a Windows nem tudja direktben futtatni (mint a php.exe-t), hanem kell hozzá egy olyan parancsfájl, amit az OS meg tud hívni, és ami átadja a PHP futtató környezetnek a PHAR fájlt. Így válik majd lehetővé, hogy a CMD ablakban műkdödjön a `composer` parancs, tehát lényegében ezt a `bat` fájlt fogja hívni. A lényeg: ebben a __composer__ mappában csinálj egy `composer.bat` nevű fájlt is, aminek a tartalma a következő egy sor legyen:
 
  ```bat
  @php "%~dp0composer.phar" %*
  ```
- Ezáltal lényegében a `composer.bat` meghívásakor a PHP értelmező futtatja a Composert, ami egy PHP fájl, átadva neki a paramétereket is (% a végén).

### Telepítések hozzáadása a Path környezeti változóhoz
Lényegében ezen a ponton majdnem kész vagy a telepítéssel, azonban jelenleg csak úgy hívható a PHP és a Composer, ha megadod az abszolút útvonalat hozzá, de ha csak annyit írsz be a parancssorba, hogy `php` vagy `composer`, arra azt írja, hogy a parancs nem található.

Ahhoz, hogy bármilyen megnyitott parancssorból egyszerűen meg lehessen hívni a `php`, illetve a `composer` parancsokat, hozzá kell a `%localappdata/Programs` mappán belül az imént elkészített két mappát adni a Path környezeti változóhoz.

Ez a legegyszerűbben úgy tehető meg, hogy beírod a Start menü keresőbe, hogy "Fiókhoz tartozó környezeti változók szerkesztése", vagy angol Windowson "Edit environment variables for your account". Alternatív lehetőség a Win+R, majd `rundll32 sysdm.cpl,EditEnvironmentVariables` parancs kiadása.

Itt fontos, hogy a megjelenő ablak fenti részében dolgozz, mivel az vonatkozik a __felhasználó__ környezeti változóira, és a labor gépeken amúgy sem lehet módosítani a rendszerhez tartozó tulajdonságokat.

**A változás érvénybe léptetéséhez az esetlegesen korábbról nyitva lévő parancssorokat be kell zárni.** A beállítások elvégzése után megnyitott parancssorok azonban már módosult Path értéket fogják átvenni, tehát azokban elérhetők lesznek a parancsok.

## Telepített eszközök tesztelése
Ha minden fent ismertetett műveletet sikeresen elvégeztél, a parancssorban elérhetővé válik a `php` és a `composer` (a már megnyitott parancssorokban nem, azokat újra meg kell nyitni), ezt így próbálhatod ki:
`php -v` és `composer -V`

Mindkét esetben ki kell írja a telepített verzió adatait, továbbá a PHP-nál meg kell jelenjen az Xdebug-ra vonatkozó információ is.

A PHP-t részletesebben úgy tudod tesztelni, ha csinálsz például egy *test.php* nevű fájlt, amibe beleírod ezt:
```php
<?php
    // PHP adatainak kiírása
    phpinfo();

    // Xdebug adatainak kiírása
    xdebug_info();
?>
```

Ezt követően nyiss egy parancssort az adott mappában, vagy cd parancssal lépj be a `test.php` mappájába, utána pedig add ki a következő parancsot: 
`php -S localhost:3000 test.php`

Ezt követően a böngészőben nyisd meg a [http://localhost:3000/](http://localhost:3000/) oldalt.

Ez egy átfogó riportot fog megjeleníteni a PHP adatairól és beállításairól, pl. láthatod a verziószámát, bekapcsolt kiegészítőket, van-e Xdebug, stb. A parancssort zárd be a végén. A parancssor bezárása értelemszerűen azt is jelenti, hogy a szerver futása is megáll.

Ugyanez a két teszt elvégezhető az `InstallTest` telepítő mappájában lévő eszközökkel is.

## Kézi telepítés menete (Linux)
Az automatikus telepítő csak Windowson működik, azonban ez a fejezet segíthet a Linuxra való telepítésben. A telepítés [ezen a leíráson](https://tecadmin.net/how-to-install-php-8-on-ubuntu-20-04/) alapul, és tesztelve lett egy Xubuntu 20.04-es rendszeren VirtualBox-ban.

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
