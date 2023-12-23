# EthicalCompany
*Making the Company more ethical.*

A mod that has the Company deliver free supplies to increase survivability and effectivity.

By providing some bare minimum equipment and adopting ethical standards, the Company discovered an unexpected benefit—increased work efficiency.

## Features
By default, the following are provided when visiting the Company building:
```
Equipment:
- Flashlight
- Walkie-Talkie

Weapons: (10% chance)
- Shovel

Credits: 60
```
The ship needs to travel to the Company at the start of a run to collect the supplies if `supplyOnCompany` is set to `true`.

## Configuration
`isCompanyEthical`: (Default: `true`) Is the company ethical? This toggles the mod.

`supplyOnCompany`: (Default: `true`) Should free goods only be given on the Company moon.

`freeWeaponChance`: (Default: `10`) Percent chance for the Company to give out arms. Set to 0 to disable.

`freeCredits`: (Default: `60`) Free credits given when arriving at the Company. Set to 0 to disable.

`supplyEquipment`: (Default: `0, 1`) IDs of equipment supplied by the Company.

`supplyWeapons`: (Default: `2`) IDs of weapons supplied by the Company.
