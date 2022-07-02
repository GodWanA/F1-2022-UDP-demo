# Jeff+ Telemetry App
### This is an open source example project how to use Codemasters UDP telemetry services, via my DLL.

##### This project currently under developement, but some of my planned features are aviable.
###### In the DLL:
  - Packet event system for all packet, and some of the newer packets emulated on older games where possible.
  - All packets last state aviable via properties.
  - Optional set limit on packages where 60Hz supported, to reduce CPU usage.
  - Async UPD listening and optional multi-core CPU support where possible.
  - Can connect any IP and port, not only localhost.

###### In the Client:
  - Livetiming system.
  - Detailed real-time driverinfo from any player in session.
  - Tool for create track layout.
  - Responsive UI where needed.

###### Planned features:
  - ###### DLL:
    - More complex custom session history packet object, what supports older games too.
    - More optional parametar for packet events.
  - ###### Client:
    - Real-time car setup viewer for all player in session where aviable.
    - Telemetry graphs
    - Laphistory preview
    - Event log
    - Option to save/load session for league players.
    - Option to export session do document format.
    - Option to share your live telemetry in real-time.
