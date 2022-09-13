
# RemoteList

Übersicht für IBS-User, welche Rechner sich im Pool der Remote-Rechner befinden und welche dieser Rechner gerade in Verwendung von einem anderen Nutzer sind.

Das Projekt befindet sich im Status: **Testphase**

## Änderungshistorie

Diese Änderungshistorie bezieht sich ausschließlich auf die readme-Datei. Alle Details zum Changelog entnehmen Sie bitte dem Absatz 'Update- und Freigabeprozess'.


| Version | Datum | Autor | Bemerkung |
| ------- | ----- | ----- | --------- |
| 1.0.0 | 17.8.2022 | FB | Ersterstellung |
| 1.1.0 | 13.9.2022 | FB | Installationsanleitung Abschnitt für User hinzugefügt |
| 1.2.0 | 13.9.2022 | FB | Code-Scanning hinzugefügt |

Änderungen in dieser Datei sind ebenfalls der git-Historie zu entnehmen.

Es ist Aufgabe der ändernden Person, Projektbeteiligte über etwaige Änderungen zu informieren. Dies betrifft insbesondere Änderungen hinsichtlich Lasten- und Pflichtenheft.

## Risikoeinteilung TISAX

Einstufung: **Normal**

- [x] Dieses Projekt ist ausschließlich für Mitarbeiter der IBS GnbH bestimmt
- [ ] Dieses Projekt verarbeitet *und* speichert Kundendaten
- [ ] Dieses Projekt verarbeitet *und* speichert Zahlungsdsaten
- [ ] Es gibt über das Internet erreichbare Schnittstellen

Die Einstufung wird folgendermaßen begründet:

- Website wird nur aus dem Intranetz erreichbar sein
- keine Kundendaten
- keine vertraulichen Informationen
- keine Informationen, die nicht sowieso für jedermann im Intranet verfügbar wären
- keine offenen Schnittstellen

## Ziel und Verwendung

Übersicht für IBS-User, welche Rechner sich im Pool der Remote-Rechner befinden und welche dieser Rechner gerade in Verwendung von einem anderen Nutzer sind.

### Auftraggeber

Das Projekt wurde in Auftrag gegeben von:

**IBS Intern**

### Beteiligte Personen

| Rolle                                   | Person                   |
| --------------------------------------- | ------------------------ |
| Projektverantwortlicher | Frank Bielecke |
| Verantwortlicher Informationssicherheit | Frank Bielecke |
| Ansprechpartner Auftraggeber | Frank Bielecke |
| Tester | Frank Bielecke |
| Verantwortlicher Git | Frank Bielecke |
| Support | Frank Bielecke |
| Verantwortlicher Dokumentation | Frank Bielecke |


### Stundenbudget

Für den Abschluss des Projektes sind 32 Stunden veranschlagt. Dies beinhaltet:
- [x] Projektleitung und -management
- [x] Softwareentwicklung
- [x] Testing
- [x] Dokumentation
- [x] Software-Wartung
- [x] Support

## Voraussetzungen



### Voraussetzungen für den Betrieb

#### Betrieb der Software

- Windows Maschine in ibs-ka Domäne
- IIS Express
- User mit Berechtigung zum Abfragen des Active Directory

#### Betriebssysteme

- keine

#### Testing

- aktueller Browser

## Installation und Deployment

- Website: Keine Installation notwendig
- Auf dem ausführenden Server muss ein Nutzer hinterlegt sein, der die "Query Information" special access permission sowie Rechte zum Starten  hat. Dazu:

1. Create a normal user via the Active Directory Users and Computers tool.
2. Add the created user to following groups Performance Monitor Users and Distributed COM Users under Builtin.
3. Open a command prompt window and execute the wmimgmt.msc command.
4. Select the Properties of WMI Control (local).
5. Select the Security tab.
6. Select Root and press the Security button.
7. Add the group Performance Monitor Users.
8. Enable all Remote Enable, Execute Methods, Enable Account and all read rights.
9. Close the add dialog and select the group Performance Monitor Users in the list.
10. Select Advanced in the Security for Root dialog and then select the group and press Edit.
11. Select This namespace and subnamespaces to grant read-only access to the whole WMI tree to this account

Im IIS auf dem Server dann Anwendungspool > Remoteliste > Erweiterte Einstellungen > Anwendungsidentität auf User ibs-ka\remoteliste stellen.



### Installation Testumgebung

- Eine Testumgebung ist nicht notwendig/sinnvoll



### Installation Entwicklungsumgebung

- Windows Maschine in ibs-ka Domäne
- Visual Studio 2022 + .NET Core 6
- User mit Berechtigung zum Abfragen des Active Directory



### Update- und Freigabeprozess

- [x] Die hier beschriebenen Prozesse triggern einen automatisierten Release.

"Veröffentlichen"-Tool in Visual Studio - dort ist ein Profil hinterlegt, dass die aktuelle Version zu VM-S-KA-REMOTE pusht.

Freigabe durch Projektverantwortlichen

### Hinweise zur Bedienung
- Bedienungsanleitung befindet sich auf der Website

### Nutzung von Fremdsoftware
- siehe NuGet-Paketmanager

## Checkliste

- [x] Machbarkeit geprüft
- [x] Lastenheft erstellt
- [x] Kostenplan erstellt
- [x] Kundenseitige Kosten wurden kalkuliert und weitergegeben
- [ ] Verbesserungsvorschläge eingeholt
- [ ] Verbesserungsvorschläge eingearbeitet
- [ ] Dokumentation erstellt
- [x] Lauffähige Version erstellt, die alle gewünschten Features enthält
- [ ] Auf Zielsystemen installiert
- [x] Bedienungsanleitung erstellt
- [x] Alle Secrets separiert von Projektstruktur



  