Der Anbieter Zentrales Netz TI MUSS den Adressbereich 2A10:1981:0000::/32 nach dem in der Tab\_Adrkonzept\_IPv6\_Test definierten Schema zur Vergabe von IPv6-Adressen an Produkttypen der TI in der Testumgebung verwenden.

Tabelle 6: Tab\_Adrkonzept\_IPv6\_Test, Adressräume IPv6 TI-Testumgebung 

<table><tr><th valign="top"><b>Netzbereich</b> </th><th valign="top"><b>Adressen</b> </th><th valign="top"><b>Netz</b> </th><th valign="top"><b>Nutzung</b> </th><th valign="top"><b>Verantwortlich</b> </th></tr>
<tr><td valign="top">TI-Testumgebung</td><td valign="top"></td><td valign="top">2A10:1981:0000::/32</td><td valign="top">TI Produktiv</td><td valign="top">Anbieter Zentrales Netz TI und GBV</td></tr>
<tr><td valign="top"><i>TI_Dezentral_TI</i></td><td valign="top"></td><td valign="top">2A10:1981:0000::/40</td><td valign="top">Dezentral TI</td><td valign="top">Anbieter Zentrales Netz TI</td></tr>
<tr><td valign="top">Konnektoren und Consumer TI</td><td valign="top"></td><td valign="top">2A10:1981:0000::/40</td><td valign="top">Konnektoren TI, Basis- u. KTR-Consumer</td><td valign="top">Anbieter Zugangsdienst, Betreiber von Basis- u. KTR-Consumer </td></tr>
<tr><td rowspan="8" valign="top">Zentrale Dienste</td><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0100::/42</td><td rowspan="2" valign="top">Zentrale Dienste<br>QoS-Klasse Platin</td><td rowspan="2" valign="top">Anbieter Zentraler Dienste</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem zentralen Dienst einen /60 Adressblock aus dem Bereich 2A10:1981:0100::/42 zu.</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0140::/42</td><td rowspan="2" valign="top">Zentrale Dienste<br>QoS-Klasse Gold</td><td rowspan="2" valign="top">Anbieter Zentraler Dienste</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem zentralen Dienst einen /60 Adressblock aus dem Bereich 2A10:1981:0140::/42 zu.</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0180::/42</td><td rowspan="2" valign="top">Zentrale Dienste<br>QoS-Klasse Silber</td><td rowspan="2" valign="top">Anbieter Zentraler Dienste</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem zentralen Dienst einen /60 Adressblock aus dem Bereich 2A10:1981:0180::/42 zu.</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:01C0::/42</td><td rowspan="2" valign="top">Zentrale Dienste<br>QoS-Klasse Bronze</td><td rowspan="2" valign="top">Anbieter Zentraler Dienste</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem zentralen Dienst einen /60 Adressblock aus dem Bereich 2A10:1981:01C0::/42 zu.</td></tr>
<tr><td rowspan="2" valign="top">VPN-Zugangsdienst</td><td valign="top">2<sup>20</sup> Netze</td><td valign="top">2A10:1981:0200::/40</td><td rowspan="2" valign="top">Anschluss VPN-Konzentratoren an die TI/SIS</td><td rowspan="2" valign="top">Anbieter Zugangsdienst</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem VPN-Zugangsdienstprovider einen /60 Adressblock aus dem Bereich 2A10:1981:0200::/40 zu.</td></tr>
<tr><td rowspan="10" valign="top">Offene Dienste</td><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0300::/42</td><td rowspan="2" valign="top">Offene Fachdienste<br>oder Dienste eines SÜV<br>QoS-Klasse Platin</td><td rowspan="2" valign="top">Anbieter Offene Fachdienste oder Dienste eines SÜV</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem Offenen Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0300::/42 zu</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0340::/42</td><td rowspan="2" valign="top">Offene Fachdienste<br>oder Dienste eines SÜV<br>QoS-Klasse Gold</td><td rowspan="2" valign="top">Anbieter Offene Fachdienste oder Dienste eines SÜV</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem Offenen Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0340::/42 zu</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0380::/42</td><td rowspan="2" valign="top">Offene Fachdienste<br>oder Dienste eines SÜV<br>QoS-Klasse Silber</td><td rowspan="2" valign="top">Anbieter Offene Fachdienste oder Dienste eines SÜV</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem Offenen Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0380::/42 zu</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:03C0::/42</td><td rowspan="2" valign="top">Offene Fachdienste<br>oder Dienste eines SÜV<br>QoS-Klasse Bronze</td><td rowspan="2" valign="top">Anbieter Offene Fachdienste oder Dienste eines SÜV</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem Offenen Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:03C0::/42 zu</td></tr>
<tr><td valign="top">2<sup>20</sup> Netze</td><td valign="top">2A10:1981:0400::/40</td><td rowspan="2" valign="top">WANDA Smart</td><td rowspan="2" valign="top">Anbieter WANDA Smart</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist den WANDA Smart Anbietern bei Bedarf einen /60 Adressblock aus dem Bereich 2A10:1981:0400::/40 zu</td></tr>
<tr><td rowspan="8" valign="top">Gesicherte Fachdienste</td><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0500::/42</td><td rowspan="2" valign="top">Gesicherte Fachdienste<br>QoS-Klasse Platin</td><td rowspan="2" valign="top">Anbieter Gesicherte Fachdienste</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem Gesicherten Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0500::/42 zu</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0540::/42</td><td rowspan="2" valign="top">Gesicherte Fachdienste<br>QoS-Klasse Gold</td><td rowspan="2" valign="top">Anbieter Gesicherte Fachdienste</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem Gesicherten Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0540::/42 zu</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:0580::/42</td><td rowspan="2" valign="top">Gesicherte Fachdienste<br>QoS-Klasse Silber</td><td rowspan="2" valign="top">Anbieter Gesicherte Fachdienste</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem Gesicherten Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:0580::/42 zu</td></tr>
<tr><td valign="top">2<sup>18</sup> Netze</td><td valign="top">2A10:1981:05C0::/42</td><td rowspan="2" valign="top">Gesicherte Fachdienste<br>QoS-Klasse Bronze</td><td rowspan="2" valign="top">Anbieter Gesicherte Fachdienste</td></tr>
<tr><td colspan="2" valign="top">Der Anbieter Zentrales Netz TI weist jedem Gesicherten Fachdienst einen /60 Adressblock aus dem Bereich 2A10:1981:05C0::/42 zu</td></tr>
<tr><td valign="top"><i>TI_Dezentral_SIS</i></td><td valign="top"></td><td valign="top">2A10:1981:0600::/40</td><td valign="top">Dezentral SIS</td><td valign="top">Anbieter Zentrales Netz TI</td></tr>
<tr><td valign="top">Konnektoren</td><td valign="top"></td><td valign="top">2A10:1981:0600::/40</td><td valign="top">Konnektoren SIS</td><td valign="top">Anbieter Zugangsdienst</td></tr>
<tr><td valign="top">TI_Betriebsreserve</td><td valign="top"></td><td valign="top">2A10:1981:0700::/40<br>bis<br>2A10:1981:FF00::/40</td><td valign="top">Reserve</td><td valign="top">Anbieter Zentrales Netz TI</td></tr>
</table>

**<=**
