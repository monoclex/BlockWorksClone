# BlockWorksClone
Block Works, but cloned so the community can revive it themselves.

# Projects

## BlockWorks.ClientCloner
Creates an exact clone of BlockWorks, ready for development.

## BlockWorks.Server (WIP)
The Serverside code needed to run BlockWorks on a PlayerIOClient server.

## BlockWorks.Server.DevStart
Starts the development server for BlockWorks.

# Getting Started
1. (optional) Host a nodejs server ( using the included service.js ) and replace REDACTED in BlockWorks.ClientCloner.Program.cs with the URL of your node // TODO: make publicly available nodejs service
2. Download the PlayerIOClient SDK and put everything in the Multiplayer folder into the root of this project
3. Download [mongoose](https://cesanta.com/binary.html)/nginx on your device to run blockworks from
4. Clone the BlockWorks Client using the BlockWorks.ClientCloner
5. Put mongoose in the "BlockWorksClientClone" folder ( or have nginx host the "BlockWorksClientClone" folder )
6. Modify BlockWorksClient.js to use the development server
7. Start the BlockWorks.Server.DevStart server
8. Help improve it?