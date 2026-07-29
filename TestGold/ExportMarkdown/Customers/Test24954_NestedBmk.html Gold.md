



**Elektronische Gesundheitskarte und Telematikinfrastruktur**




**Übergreifende Spezifikation**\
**Netzwerk**


{include-macros:/macros.gematikMacros}

#renderGematikDokumentenTitel($document)
# <a name="polarion_wiki-15071"></a>**Dokumentinformationen**
**Änderungen zur Vorversion**

Anpassungen des vorliegenden Dokumentes im Vergleich zur Vorversion können Sie der nachfolgenden Tabelle entnehmen.

Bitte beachten Sie die Hinweise zur Einführung der Benennungen 'WANDA Basic' und 'WANDA Smart' (siehe Dokumentenhistorie).

**Dokumentenhistorie**
# <a name="polarion_wiki-15072"></a>**Inhaltsverzeichnis**
[Dokumentinformationen](#polarion_wiki-15071)

[Inhaltsverzeichnis](#polarion_wiki-15072)

: [1 Einordnung des Dokuments](#polarion_wiki-15073)

: : [1.1 Zielsetzung](#polarion_wiki-15074)

: : [1.2 Zielgruppe](#polarion_wiki-15075)

: : [1.3 Geltungsbereich](#polarion_wiki-15076)

: : [1.4 Abgrenzung des Dokuments](#polarion_wiki-15077)

: : [1.5 Methodik](#polarion_wiki-15078)

: [2 Übergreifende Netzwerk-Festlegungen](#polarion_wiki-15079)

: : [2.1 Netztopologie](#polarion_wiki-15080)

: : [2.2 Netzwerkprotokolle](#polarion_wiki-15081)

: : : [2.2.1 OSI-Schicht 1 und 2 (Physical/Data Link)](#polarion_wiki-15082)

: : : [2.2.2 OSI-Schicht 3 (Network)](#polarion_wiki-15083)

: : : : [2.2.2.1 IP-Version 4](#polarion_wiki-15084)

: : : : [2.2.2.2 IP-Version 6](#polarion_wiki-15085)

: : : [2.2.3 OSI-Schicht 4 (Transport)](#polarion_wiki-15086)

: : : : [2.2.3.1 Transmission Control Protocol (TCP) und User Datagram Protocol (UDP)](#polarion_wiki-15087)

: : : : [2.2.3.2 UDP/TCP-Portbereiche](#polarion_wiki-15088)

: : : : [2.2.3.3 Transport Layer Security (TLS)](#polarion_wiki-15089)
## <a name="polarion_wiki-15073"></a>**1 Einordnung des Dokuments**
### <a name="polarion_wiki-15074"></a>**1.1 Zielsetzung**
Die Spezifikation Netzwerk definiert die Rahmenbedingungen und trifft die übergreifenden Festlegungen zum Netzwerk, dem Namensdienst und dem Zeitdienst in der TI. Dabei werden die für den Wirkbetrieb der TI erforderlichen Anforderungen an die Netzinfrastruktur berücksichtigt, eine Erweiterbarkeit um künftige Anwendungen jedoch beachtet.

Die übergreifende Spezifikation Netzwerk behandelt folgende inhaltlichen Schwerpunkte:

- Netztopologie und Netzumgebungen
- Vorgaben zu grundlegenden Netzwerkprotokollen
- IP-Adresskonzept – Definition von Adressbereichen
- IP-Routingkonzept
- Priorisierung auf Netzwerkebene
- Vorgaben zu Sicherheitskomponenten
- Namenskonzept – Vorgaben zu Namensräumen und DNS
- Vorgaben zum Zeitdienst
### <a name="polarion_wiki-15075"></a>**1.2 Zielgruppe**
Das Dokument richtet sich an Hersteller und Anbieter von netzwerkfähigen Produkten der TI.
### <a name="polarion_wiki-15076"></a>**1.3 Geltungsbereich**
Dieses Dokument enthält normative Festlegungen zur Telematikinfrastruktur des deutschen Gesundheitswesens. Der Gültigkeitszeitraum der vorliegenden Version und deren Anwendung in Zulassungsverfahren wird durch die gematik GmbH in gesonderten Dokumenten (z. B. Dokumentenlandkarte, Produkttypsteckbrief, Leistungsbeschreibung) festgelegt und bekannt gegeben.

***Schutzrechts-/Patentrechtshinweis***

*Die nachfolgende Spezifikation ist von der gematik allein unter technischen Gesichtspunkten erstellt worden. Im Einzelfall kann nicht ausgeschlossen werden, dass die Implementierung der Spezifikation in technische Schutzrechte Dritter eingreift. Es ist allein Sache des Anbieters oder Herstellers, durch geeignete Maßnahmen dafür Sorge zu tragen, dass von ihm aufgrund der Spezifikation angebotene Produkte und/oder Leistungen nicht gegen Schutzrechte Dritter verstoßen und sich ggf. die erforderlichen Erlaubnisse/Lizenzen von den betroffenen Schutzrechtsinhabern einzuholen. Die gematik GmbH übernimmt insofern keinerlei Gewährleistungen.*
### <a name="polarion_wiki-15077"></a>**1.4 Abgrenzung des Dokuments**
Festlegungen zu der Netzwerkkomponente VPN-Zugangsdienst erfolgen in [gemSpec\_VPN\_ZugD].

Die Festlegung der spezifischen Anbindungen von Komponenten an die Netzinfrastruktur der TI und die Einbindung der Netzdienste erfolgen auf der Basis dieser übergreifenden Spezifikation in den jeweiligen Spezifikationen der Produkttypen.
### <a name="polarion_wiki-15078"></a>**1.5 Methodik**
Anforderungen als Ausdruck normativer Festlegungen werden durch eine eindeutige ID sowie die dem RFC 2119 [RFC2119] entsprechenden, in Großbuchstaben geschriebenen deutschen Schlüsselworte MUSS, DARF NICHT, SOLL, SOLL NICHT, KANN gekennzeichnet.

Sie werden im Dokument wie folgt dargestellt:

**<AFO-ID> - <Titel der Afo>**\
Text / Beschreibung\
**[**<=**]**

Dabei umfasst die Anforderung sämtliche zwischen Afo-ID und Textmarke angeführten Inhalte.
## <a name="polarion_wiki-15079"></a>**2 Übergreifende Netzwerk-Festlegungen**
### <a name="polarion_wiki-15080"></a>**2.1 Netztopologie**
In diesem Kapitel wird die grundlegende Netztopologie der TI dargestellt um einen Überblick der beteiligten Systeme auf der Netzwerkebene zu geben. In den Spezifikationen der jeweiligen Produkttypen erfolgt, wo notwendig, eine detaillierte Darstellung der einzusetzenden Netztopologie.

Die Abb\_NetzTopologie\_Schema zeigt eine schematische Übersicht zur Netztopologie der TI auf logischer Ebene, die sich an den in der Gesamtarchitektur definierten Zonen orientiert.

Abbildung 1: Abb\_NetzTopologie\_Schema, Netztopologie der TI 

In Abb\_NetzTopologie\_Detail wird auf einer detaillierteren Netzwerkebene die mögliche Verteilung von an der TI-Plattform angebundenen Produkttypen dargestellt (ohne Secure Internet Service (SIS)).

Der Adressat „weitere Anwendungen des Gesundheitswesens“ umfasst die Anwendungskategorien WANDA Basic und WANDA Smart.

Der Adressat „weitere Anwendungen des Gesundheitswesens mit Zugriff auf Dienste der TI“ wird durch die Anwendungskategorien WANDA Smart und der Adressat „weitere Anwendungen des Gesundheitswesens ohne Zugriff auf Dienste der TI“ durch die Anwendungskategorie WANDA Basic beschrieben.

Abbildung 2: Abb\_NetzTopologie\_Detail, Netzwerktopologie der TI - detailliert 
### **2.2 Netzwerkprotokolle**
#### <a name="polarion_wiki-15082"></a>**2.2.1 OSI-Schicht 1 und 2 (Physical/Data Link)**
<a name="polarion_wiki-15081"></a><a name="polarion_wiki-15083"></a>**GS-A\_4009**

<a name="polarion_wiki-15084"></a>Alle Produkttypen der TI und Anbieter weiterer Anwendungen des Gesundheitswesens mit Zugriff auf Dienste der TI MÜSSEN beim Einsatz des Ethernet-Protokolls an Schnittstellen zwischen Produkttypen der TI die Einhaltung der [IEEE 802.3] sicherstellen. \
**<=**
#### **2.2.2 OSI-Schicht 3 (Network)**
Als produktiv eingesetztes Netzwerkprotokoll auf der OSI-Schicht 3 wird in der TI das Internetprotokoll in der Version 4 (IPv4) eingesetzt. Als Teil der laufenden Migration wird bei definierten Produkttypen bereits die Unterstützung des Internetprotokolls in der Version 6 (IPv6) gefordert. Vorgaben zum Protokoll Encapsulation Security Payload (ESP) werden in [gemSpec\_VPN\_ZugD] definiert.
##### **2.2.2.1 IP-Version 4**
**GS-A\_4831**

Produkttypen der TI und weitere Anwendungen des Gesundheitswesens MÜSSEN mindestens die in Tab\_Standards\_IPv4 aufgeführten Standards unterstützen.

Tabelle 1: Tab\_Standards\_IPv4, Standards IPv4 

**GS-A\_4832**

Produkttypen der TI und andere Anwendungen des Gesundheitswesens MÜSSEN sicherstellen, dass Path MTU Discovery (PMTUD) gemäß [RFC1191] im gesamten Netzwerk funktioniert. Insbesondere MÜSSEN Router und Gateways die erforderlichen ICMP-Messages erzeugen, und Sicherheitsgateways MÜSSEN diese ICMP-Messages passieren lassen. Anfragen durch einen ICMP-Request MÜSSEN mit einem ICMP-Reply beantwortet werden. \
**<=**
##### <a name="polarion_wiki-15085"></a>**2.2.2.2 IP-Version 6**
**GS-A\_4010**

Produkttypen, die zentrale Dienste der TI-Plattform bereitstellen, MÜSSEN die in [RIPE-772]  für die jeweilige Geräteklasse unter Mandatory Support aufgeführten Anforderungen erfüllen.\
\
**<=**

**GS-A\_4011**

Zentrale Dienste der TI-Plattform MÜSSEN IPv4 und IPv6 parallel als Protokoll (Dual-Stack-Mode) unterstützen. Die TSP X.509 SOLLEN IPv4 und IPv6 parallel unterstützen.

**<=**

**GS-A\_4012**

Produkttypen, die zentrale Dienste der TI-Plattform bereitstellen, MÜSSEN IPv4 und IPv6 als Protokoll unterstützen, wobei für beide Protokolle eine vergleichbare Leistung vorhanden sein muss, d. h. weniger als 15% Unterschied zwischen den beiden Protokollen bei Input, Output, Durchsatz, Weiterleitung und Verarbeitung. \
​​

**<=**

**A\_17824**

Zentrale Dienste der TI-Plattform MÜSSEN an ihren Außenschnittstellen zu anderen Komponenten und Diensten der TI sowie WANDA Basic und WANDA Smart im zentralen Netz der TI und im Internet IPv4 und IPv6 parallel als Protokoll im Dual-Stack-Mode nutzen. **<=**
#### <a name="polarion_wiki-15086"></a>**2.2.3 OSI-Schicht 4 (Transport)**
##### <a name="polarion_wiki-15087"></a>**2.2.3.1 Transmission Control Protocol (TCP) und User Datagram Protocol (UDP)**
Für die Implementierung von TCP und UDP werden an dieser Stelle keine normativen Vorgaben erhoben. Es wird empfohlen Implementierungen von TCP/IP-Stacks zu nutzen, die aktuelle Verfahren zur Übertragung und Steuerung von Daten einsetzen.
##### <a name="polarion_wiki-15088"></a>**2.2.3.2 UDP/TCP-Portbereiche**
Für die Verwaltung und Dokumentation von UDP/TCP-Portbereichen ist in der TI ein übergreifender Prozess zu etablieren, der durch den Anbieter Zentrales Netz TI implementiert und vom Gesamtbetriebsverantwortlichen (GBV) freigegeben wird.

In den folgenden Anforderungen werden die Verantwortlichkeiten und weitere Vorgaben zum Prozess „Verwaltung von UDP/TCP-Portbereichen“ definiert.

**GS-A\_4833**

Der Anbieter Zentrales Netz TI MUSS den Prozess „Verwaltung von UDP/TCP-Portbereichen“ mit den folgenden Inhalten definieren und implementieren:

- Erstellung und Pflege eines Vergabeschemas für UDP/TCP-Portbereiche
- Operative Vergabe von UDP/TCP-Portbereichen
- Erstellung und Pflege von Dokumentations- und Reportingschemas
- Dokumentation und Reporting von UDP/TCP-Portbereichen

Der Anbieter Zentrales Netz TI ist der Verantwortliche für den gesamten Prozess.

**<=**

**GS-A\_4886**

Der GBV MUSS den vom Anbieter Zentrales Netz TI definierten Prozess „Verwaltung von UDP/TCP-Portbereichen“ freigeben.

**<=**

**GS-A\_4014**

Der GBV MUSS für die Zuteilung von UDP/TCP-Portbereichen ein Vergabeschema unter Berücksichtung der Dienstklassen zur Netzwerkpriorisierung erstellen und dem Anbieter Zentrales Netz TI zur Verfügung stellen.\
Der GBV MUSS das Vergabeschema für UDP/TCP-Portbereiche auf Grundlage des [RFC6335] erstellen. Der GBV MUSS für die Vergabe von UDP/TCP-Portbereichen den in [RFC6335] definierten Bereich von 49152-65535 (Dynamic/Private Ports) nutzen. Hiervon ausgenommen sind Anwendungen die in [RFC6335] definierte Bereiche der System Ports (Well-Known Ports) bzw. User Ports (Registered Ports) nutzen.\
**<=**

**GS-A\_4016**

Der Anbieter Zentrales Netz TI MUSS UDP/TCP-Portbereiche nach den Vorgaben des Vergabeschemas an die einzelnen Anbieter der Produkttypen der TI bedarfsgerecht zuweisen. Die Vergabe der UDP/TCP-Portbereiche erfolgt im Rahmen des Test- und Zulassungsverfahrens von Anbietern eines Produkttyps.

**<=**

**GS-A\_4013**

Produkttypen von Fachanwendungen und Zentralen Diensten der TI-Plattform und Anbieter weiterer Anwendungen des Gesundheitswesens mit Zugriff auf Dienste der TI MÜSSEN die zugeordneten bzw. abgestimmten UDP/TCP-Portbereiche für die Kommunikation in der TI nutzen.\
**<=**

**GS-A\_4753**

Der GBV MUSS in Abstimmung mit dem Anbieter Zentrales Netz TI das Dokumentationsformat für die UDP/TCP-Portbereiche festlegen und dem Anbieter von Produkttypen der TI zur Verfügung stellen.

**<=**

**GS-A\_4017**

Der Anbieter Zentrales Netz TI MUSS die Vergabe der UDP/TCP-Portbereiche dokumentieren und diese Dokumentation dem GBV bei Änderungen und auf Anforderung zur Verfügung stellen.

**<=**

**GS-A\_4018**

Die Anbieter von Produkttypen der TI und Anbieter weiterer Anwendungen des Gesundheitswesens mit Zugriff auf Dienste der TI MÜSSEN die Nutzung der zugeteilten und mit den Anbietern weiterer Anwendungen des Gesundheitswesens mit Zugriff auf Dienste der TI abgestimmten UDP/TCP-Portbereiche dokumentieren und diese Dokumentation dem Anbieter Zentrales Netz TI bei Änderungen und auf Anforderung zur Verfügung stellen.\
**<=**
##### <a name="polarion_wiki-15089"></a>**2.2.3.3 Transport Layer Security (TLS)**
Anforderungen zu den einzusetzenden kryptographischen Verfahren für TLS und daraus folgende resultierende Vorgaben zur TLS-Version werden in [gemSpec\_Krypt] definiert.

Weitere Eigenschaften und Funktionen für das TLS-Protokoll können wo notwendig in den Spezifikationen von Produkttypen festgelegt werden.
