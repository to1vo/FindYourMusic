# FindYourMusic - Fullstack projekti

ASP .NET Core MVC, C#, HTML5/CSS3, JavaScript, MySQL + Entity Framework Core

## Sovelluksen kuvaus
Web-sovellus, jossa käyttäjät voivat kuvailla kappaleita erilaisilla valmiiksi määritetyilla kategorioilla. Muut käyttäjät voivat löytää kappaleita tekemällä hakuja valiten tiettyjä kategorioita. Kategorioiden teemat eivät ole niinkään genre tai musiikkityyli kohtaisia, vaan enemmänkin tunteisiin, ympäristöön ja äänimaailmaan liittyviä.

## Toiminnot
### Normaali käyttäjä
- Kirjautuminen tai rekisteröityminen
- Kappaleiden etsiminen kategorioilla
- Kappaleiden tietojen tarkastelu

### Kirjautunut käyttäjä
- Kappaleiden etsiminen kategorioilla
- Kappaleiden tietojen tarkastelu
- Kappaleiden kuvaileminen kategorioilla
- Kappaleen lisäys tietokantaan manuaalisesti
- Kappaleiden lisääminen kirjanmerkkeihin
- Oman profiilin tarkastelu

### Kappaleiden haku
Käyttäjä voi hakea siis sivuston tietokannasta kappaleita tietyllä määrällä kategorioita. Hakutuloksiin tulee siis näkyviin vain sellaiset kappaleet, joihin on tehty kuvauksia. Hakutuloksien järjestys määräytyy sen perusteella, kuinka moni kategorioista sopii sekä kuinka usein sitä on annettu kyseiselle kappaleelle.

### Kappaleiden tietojen tarkastelu
Klikkaamalla kappaletta vie se kappaleen omalle sivulle, jossa näkyy siihen liittyviä perustietoja sekä mahdollisesti oma kuvaus.

### Kirjanmerkiksi lisääminen
Käyttäjä voi painaa kirjanmerkki kuvaketta kappaleiden vierestä eri näkymissä, tämä lisää sen käyttäjän omalle "muista kuunnella" listalle. Käyttäjä voi tietenkin myös halutessaan poistaa kappaleen listalta painamalla samaa kuvaketta uudestaan.

### Kappaleiden kuvaileminen
Käyttäjä tekee ensin haun haluamastaan kappaleesta nimen tai artistin perusteella, tulokset tulevat rajapinnasta sekä tietokannasta. Jos käyttäjä ei löydä haluamaan, mahdollisesti harvinaista kappaletta voi hän lisätä sen manuaalisesti tietokantaan. Valittuaan kappaleen käyttäjä valitsee mielestään siihen sopivat kategoriat (max neljä), jotka kuvaavat kappaleeseen sopivaa tunnetta, ympäristöä tai äänimaailmaa. Käyttäjä voi muuttaa omia kategorisointejaan, joko kyseisen kappaleen sivun kautta, oman profiilin kautta tai hakemalla kappaleen kokonaan uudelleen. Kun käyttäjä menee kuvailemaan jo aikaisemmin kuvailtua kappaletta, sen hetkiset kategoriat ovat automaattisesti valittuna. 

### Kappaleen lisäys tietokantaan
Kuvaillut kappaleet tallennetaan tietokantaan. Eli kun käyttäjä etsii kappaleita kategorioilla tulokset tulevat tietokannasta. Manuaalisesti lisätessä käyttäjä lisää tarvittavat tiedot haluamastaan kappaleesta, mahdollinen duplikaatti tarkastus suoritetaan ennen kappaleen lisäystä tietokantaan.

## Tekninen toteutus
Sovellus on siis toteutettu ASP .NET Core MVC ohjelmistokehyksellä, joten pääohjelmointikielenä toimi C#. Web-sivun dynaamiset toiminnot on toteutettu Vanilla JavaScriptillä. Tietokantana toimii MySQL ja sitä käsitellään Entity Framework Core kehyksellä. 

### Tiedon haku
Koska sovellus käsittelee kappaleita ja niihin liittyvää tietoa, hyödyntää se kolmannen osapuolen rajapintaa. Rajapinnaksi valikoitui [last.fm API](https://www.last.fm/api). Sovellus hyödyntää rajapintaa kappaleiden hakemiseen sekä yksittäisen kappaleen tietojen tallentamiseen. Osa kappaleen tiedoista tallennetaan sovelluksen tietokantaan, jotta sivuston toiminnallisuus ei ole vain rajapinnan varassa. Kyseisen rajapinnan tulokset saattaavat olla välillä puutteellisia, joten se piti ottaa huomioon sitä käyttäessä.

### Tiedon tallennus
Sovelluksen tietokantana toimii MySQL.
- <b>User</b>: Käyttäjänimi sekä enkryptattu salasana.
- <b>Track</b>: Kategorisoiduista kappaleista tallennetaan tärkeimmät tiedot. Rajapinnasta tulleet sekä käyttäjien manuaalisesti lisäämät.
- <b>Category</b>: Kategoriat, joilla kappaleita kuvaillaan on tallennettuna tietokantaan.
- <b>CategoryGroup</b>: Kategoria ryhmien nimet on tallennettuna tietokantaan, jotta yksittäiset kategoriat voidaan helposti liittää oikeaan ryhmään.
- <b>UserTrackCategory</b>: Tämä on viitetaulu, johon tallentuu käyttäjän kuvaus kappaleesta per kategoria.
- <b>Bookmark</b>: Tähän viitetauluun tallentuu käyttäjien kirjanmerkatut kappaleet.

![Er-kaavio](https://github.com/to1vo/FindYourMusic/blob/main/er-kaavio.png)

### APIService
Service, jonka avulla voi keskustella rajapinnan kanssa. Methodit kappaleiden hakua sekä kappaleen tietoja varten, jotka käyttävät last.fm API:n track.search ja track.getInfo endpointteja.

### CategoryService
Sisältää muutaman methodin kategorioiden ja niiden ryhmien tuomiseen tietokannasta, nopeuttaen näin prosessia, joka on aika yleinen.

### TrackService
Erilaisia methodeja kappaleiden hakemiseen tietokannasta tai tarkastuksia siitä löytyykö kappaletta tietokannasta.

### UserService
Methodeja joiden avulla voi hakea käyttäjään liittyvää dataa tietokannasta (esim. kaikki kirjanmerkatut kappaleet). Sisältää myös toimintoja uuden käyttäjän luomiseen sekä jo olemassa olevan muokkaamiseen.

### Etusivu - HomeController
Hyödyntää Track- ja UserServiceä hakien näin etusivulla näkyvillä olevan datan. Sivulla näkyvä data muuttuu sen perusteella onko käyttäjä kirjautuneena sisään.

### About sivu - AboutController
Palauttaa viewin.

### Autentikaatio - AuthenticationController 
Tämän kontrollerin tarkoitus on hallita käyttäjän autentikaatiota, eli kirjautumista sekä uuden käyttäjän luontia. Kontrolleri käyttää tässä apuna UserServiceä. Sisäänkirjautuminen hyödyntää CookieAuthenticationDefaultsia sekä ClaimsIdentityä, eli luo cookien, jolla se pitää käyttäjän kirjautumissession tallessa. Identityyn on tallennettuna käyttäjän nimi sekä id, jonka avulla kirjautuneen käyttäjän dataa voidaan hakea tietokannasta id:n avulla.

### Kappaleiden haku - SearchController
Hallitsee käyttäjän hakua valituilla kategorioilla hyödyntäen Category-, Track- sekä UserServiceä. Kun käyttäjä hakee kappaleita valituilla kategorioilla tulokset tulevat tietokannasta, koska sinne on tallennettu kaikki kappaleet joita joku on kuvaillut.

### Kappaleen kuvaus - DescribeController
Tämän kontrollerin endpointit ovat käytössä vain kirjautuneelle käyttäjälle ja se hyödyntää API-, Category-, Track- sekä UserServiceä. Hallitsee kappaleen kuvaus sivua, hakee yksittäisen kappaleen tiedot API:sta, tuo mahdollisesti käyttäjän edellisen kuvauksen, lisää uuden kuvauksen kappaleelle sekä suorittaa kappaleen manuaalisen lisäyksen.

### Kappale sivu - TrackController
Hakee kyseisen kappaleen tiedot tietokannasta sekä siihen liittyvää dataa (esim. samankaltaiset kappaleet, yleisimmät kategoriat), sisältää myös endpointit kirjanmerkin lisäykseen hyödyntäen näissä kaikissa Track- ja UserServiceä. Näkyvä sivu eroaa hieman siitä onko käyttäjä kirjautuneena. Kirjautunut käyttäjä pystyy lisäämään kappaleen kirjanmerkkeihin sekä nähdä oman kuvauksen kyseisestä kappaleesta, josta hän pääsee myös muokkaamaan sitä.

### Käyttäjän profiili - UserController
Käyttää UserServiceä tuoden näkyviin kirjautuneeseen käyttäjään liittyvää dataa (esim. viimeisimmät kuvaukset, käytetyimmät kategoriat). Sisältää neljä eri viewiä: profiili, kirjanmerkit, kuvaukset sekä käyttäjänimen muokkaus.

### PaginatedList
Luokka, joka perii List luokan. Eli lista objekti, joka jakaa listan itemit sivuihin ja antaa tiedot maksimi sivumäärästä, sekä siitä onko seuraavaa tai edellistä sivua. Hyödynsin tätä muutamassa eri viewissä, jossa voi mahdollisesti olla paljon dataa, tällöin kohteet jakautuvat sivuihin. 
