FROM mono:onbuild

RUN mkdir /configuration

VOLUME /configuration
VOLUME /openvpn

COPY openvpn-status2mqtt.json /configuration/


CMD [ "mono",  "./openvpn-status2mqtt.exe" ]
