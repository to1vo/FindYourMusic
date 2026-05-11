# FindYourMusic - Fullstack projekti

ASP .NET Core MVC, C#, HTML5/CSS3, JavaScript, MySQL + Entity Framework Core

## Sovelluksen kuvaus
Web-sovellus, jossa käyttäjät voivat kuvailla kappaleita erilaisilla valmiiksi määritetyilla kategorioilla. Muut käyttäjät voivat löytää kappaleita tekemällä hakuja valiten tiettyjä kategorioita. Kategorioiden teemat eivät ole niinkään genre tai musiikkityyli kohtaisia, vaan enemmänkin tunteisiin, tilanteeseen ja äänimaailmaan liittyviä.

## Toiminnot
### Normaali käyttäjä
- Kirjautuminen tai rekisteröityminen
- Kappaleiden etsiminen kategorioilla
- Kappaleiden tietojen tarkastelu

### Kirjautunut käyttäjä
- Kappaleiden etsiminen kategorioilla
- Kappaleiden tietojen tarkastelu
- Kappaleiden kuvaileminen kategorioilla
- Kappaleen lisäys tietokantaan
- Kappaleiden lisääminen kirjanmerkkeihin
- Oman profiilin tarkastelu

### Kappaleiden haku
Käyttäjä voi hakea siis sivuston tietokannasta kappaleita tietyllä määrällä kategorioita. Hakutuloksiin tulee siis näkyviin vain sellaiset kappaleet, joihin on tehty kuvauksia. Hakutuloksien järjestys määräytyy sen perusteella, kuinka moni kategorioista sopii sekä kuinka usein sitä on annettu kyseiselle kappaleelle.

### Kappaleiden tietojen tarkastelu
Klikkaamalla kappaletta vie se kappaleen omalle sivulle, jossa näkyy siihen liittyviä perustietoja sekä mahdollisesti oma kuvaus.

### Kirjanmerkiksi lisääminen
Käyttäjä voi painaa kirjanmerkki kuvaketta kappaleiden vierestä eri näkymissä, tämä lisää sen käyttäjän omalle kuuntelulistalle. Käyttäjä voi tietenkin myös halutessaan poistaa kappaleen kuuntelulistalta painamalla samaa kuvaketta uudestaan.

### Kappaleiden kuvaileminen
Käyttäjä tekee ensin haun haluamastaan kappaleesta nimen tai artistin perusteella, tulokset tulevat rajapinnasta sekä tietokannasta. Jos käyttäjä ei löydä haluamaan, mahdollisesti harvinaista kappaletta voi hän lisätä sen manuaalisesti. Valittuaan kappaleen käyttäjä valitsee mielestään siihen sopivat kategoriat (max neljä), jotka kuvaavat kappaleeseen sopivaa tunnetta, ympäristöä tai äänimaailmaa. Käyttäjä voi muuttaa omia kategorisointejaan, joko kyseisen kappaleen sivun kautta, oman profiilin kautta tai hakemalla kappaleen kokonaan uudelleen. Kun käyttäjä menee kuvailemaan jo aikaisemmin kuvailtua kappaletta, sen hetkiset kategoriat ovat automaattisesti valittuna. 

### Kappaleen lisäys tietokantaan
Käyttäjä lisää tarvittavat tiedot haluamastaan kappaleesta, mahdollinen duplikaatti tarkastus suoritetaan ennen sen lisäystä tietokantaan.

## Tiedon haku ja tallennus
### Tiedon haku
Koska sovellus käsittelee kappaleita ja niihin liittyvää tietoa, hyödyntää se kolmannen osapuolen rajapintaa. Rajapinnaksi valikoitui [last.fm API](https://www.last.fm/api). Sovellus hyödyntää rajapintaa kappaleiden löytämiseen sekä tietojen tallentamiseen. Osa kappaleen tiedoista tallennetaan sovelluksen tietokantaan, jotta sivuston toiminnallisuus ei ole vain rajapinnan varassa. Kyseisen rajapinnan tulokset saattaavat olla välillä puutteellisia, joten se piti ottaa huomioon sitä käyttäessä.

### Tiedon tallennus
Sovelluksen tietokantana toimii MySQL.
- Käyttäjät: Käyttäjänimi sekä enkryptattu salasana.
- Kappaleet: Kategorisoiduista kappaleista tallennetaan tärkeimmät tiedot. Rajapinnasta sekä käyttäjien manuaalisesti lisäämät.
- Kategoria: Kategoriat, joilla kappaleita kuvaillaan on tallennettuna tietokantaan.
- Kategoria ryhmä: Kategoria ryhmien nimet on tallennettuna tietokantaan, jotta yksittäiset kategoriat voidaan helposti liittää oikeaan ryhmään.
- Käyttäjän kuvaus kappaleesta: Tämä on viitetaulu, johon on tallennettu käyttäjän id, kategorian id, kappaleen id sekä päivämäärä.
- 