# tty2ip
Very simple redirector that will send raw bytes to and from serial port over tcp/ip connection

Docker image for Armbian (armhf) [maxbl4/tty2ip:arm](https://hub.docker.com/r/maxbl4/tty2ip/)

Motivation: I wanted to be able to talk to RFID module with UART TTL interface. 
I connected it to orange PI, but wanted to develop and test on the laptop.
Checked the linux forwarding abilities with ser2net, socat, netcat. 
It seems, they all alter binary traffic in some way, so RFID module did not respond.
Spent a couple of hours to build this redirector and it worked flawlessly.
This is by no means a solution for production, but should work well for ad-hoc testing and experiments.
