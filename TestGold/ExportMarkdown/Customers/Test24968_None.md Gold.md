Der Anbieter Zentrales Netz TI MUSS den Adressbereich 2A10:1981:0000::/32 nach dem in der Tab\_Adrkonzept\_IPv6\_Test definierten Schema zur Vergabe von IPv6-Adressen an Produkttypen der TI in der Testumgebung verwenden.

Tabelle 6: Tab\_Adrkonzept\_IPv6\_Test, Adressräume IPv6 TI-Testumgebung 

|**Netzbereich** |**Adressen** |**Netz** |**Nutzung** |**Verantwortlich** |
| :- | :- | :- | :- | :- |
|TI-Testumgebung||2A10:1981:0000::/32|TI Produktiv|Anbieter Zentrales Netz TI und GBV|
|*TI\_Dezentral\_TI*||2A10:1981:0000::/40|Dezentral TI|Anbieter Zentrales Netz TI|
|Konnektoren und Consumer TI||2A10:1981:0000::/40|Konnektoren TI, Basis- u. KTR-Consumer|Anbieter Zugangsdienst, Betreiber von Basis- u. KTR-Consumer |
|Zentrale Dienste|2<sup>18</sup> Netze|2A10:1981:0100::/42|Zentrale Dienste<br>QoS-Klasse Platin|Anbieter Zentraler Dienste|
||Der Anbieter Zentrales Netz TI weist jedem zentralen Dienst einen /60 Adressblock aus dem Bereich 2A10:1981:0100::/42 zu.||||
||2<sup>18</sup> Netze|2A10:1981:0140::/42|Zentrale Dienste<br>QoS-Klasse Gold|Anbieter Zentraler Dienste|
||Der Anbieter Zentrales Netz TI weist jedem zentralen Dienst einen /60 Adressblock aus dem Bereich 2A10:1981:0140::/42 zu.||||
||2<sup>18</sup> Netze|2A10:1981:0180::/42|Zentrale Dienste<br>QoS-Klasse Silber|Anbieter Zentraler Dienste|
||Der Anbieter Zentrales Netz TI weist jedem zentralen Dienst einen /60 Adressblock aus dem Bereich 2A10:1981:0180::/42 zu.||||
||2<sup>18</sup> Netze|2A10:1981:01C0::/42|Zentrale Dienste<br>QoS-Klasse Bronze|Anbieter Zentraler Dienste|
||Der Anbieter Zentrales Netz TI weist jedem zentralen Dienst einen /60 Adressblock aus dem Bereich 2A10:1981:01C0::/42 zu.||||
|VPN-Zugangsdienst|2<sup>20</sup> Netze|2A10:1981:0200::/40|Anschluss VPN-Konzentratoren an die TI/SIS|Anbieter Zugangsdienst|
||Der Anbieter Zentrales Netz TI weist jedem VPN-Zugangsdienstprovider einen /60 Adressblock aus dem Bereich 2A10:1981:0200::/40 zu.||||
|Offene Dienste|2<sup>18</sup> Netze|2A10:1981:0300::/42|Offene Fachdienste<br>oder Dienste eines SÜV<br>QoS-Klasse Platin|Anbieter Offene Fachdienste oder Dienste eines SÜV|
||Der Anbieter Zentrales Netz TI weist jedem Offenen Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0300::/42 zu||||
||2<sup>18</sup> Netze|2A10:1981:0340::/42|Offene Fachdienste<br>oder Dienste eines SÜV<br>QoS-Klasse Gold|Anbieter Offene Fachdienste oder Dienste eines SÜV|
||Der Anbieter Zentrales Netz TI weist jedem Offenen Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0340::/42 zu||||
||2<sup>18</sup> Netze|2A10:1981:0380::/42|Offene Fachdienste<br>oder Dienste eines SÜV<br>QoS-Klasse Silber|Anbieter Offene Fachdienste oder Dienste eines SÜV|
||Der Anbieter Zentrales Netz TI weist jedem Offenen Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0380::/42 zu||||
||2<sup>18</sup> Netze|2A10:1981:03C0::/42|Offene Fachdienste<br>oder Dienste eines SÜV<br>QoS-Klasse Bronze|Anbieter Offene Fachdienste oder Dienste eines SÜV|
||Der Anbieter Zentrales Netz TI weist jedem Offenen Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:03C0::/42 zu||||
||2<sup>20</sup> Netze|2A10:1981:0400::/40|WANDA Smart|Anbieter WANDA Smart|
||Der Anbieter Zentrales Netz TI weist den WANDA Smart Anbietern bei Bedarf einen /60 Adressblock aus dem Bereich 2A10:1981:0400::/40 zu||||
|Gesicherte Fachdienste|2<sup>18</sup> Netze|2A10:1981:0500::/42|Gesicherte Fachdienste<br>QoS-Klasse Platin|Anbieter Gesicherte Fachdienste|
||Der Anbieter Zentrales Netz TI weist jedem Gesicherten Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0500::/42 zu||||
||2<sup>18</sup> Netze|2A10:1981:0540::/42|Gesicherte Fachdienste<br>QoS-Klasse Gold|Anbieter Gesicherte Fachdienste|
||Der Anbieter Zentrales Netz TI weist jedem Gesicherten Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0540::/42 zu||||
||2<sup>18</sup> Netze|2A10:1981:0580::/42|Gesicherte Fachdienste<br>QoS-Klasse Silber|Anbieter Gesicherte Fachdienste|
||Der Anbieter Zentrales Netz TI weist jedem Gesicherten Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0580::/42 zu||||
||2<sup>18</sup> Netze|2A10:1981:05C0::/42|Gesicherte Fachdienste<br>QoS-Klasse Bronze|Anbieter Gesicherte Fachdienste|
||Der Anbieter Zentrales Netz TI weist jedem Gesicherten Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:05C0::/42 zu||||
|*TI\_Dezentral\_SIS*||2A10:1981:0600::/40|Dezentral SIS|Anbieter Zentrales Netz TI|
|Konnektoren||2A10:1981:0600::/40|Konnektoren SIS|Anbieter Zugangsdienst|
|TI\_Betriebsreserve||2A10:1981:0700::/40<br>bis<br>2A10:1981:FF00::/40|Reserve|Anbieter Zentrales Netz TI|

**<=**
