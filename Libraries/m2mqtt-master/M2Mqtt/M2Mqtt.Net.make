

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=../../../../..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:TRACE;DEBUG;SSL"
ASSEMBLY = ../bin/Debug/M2Mqtt.Net/M2Mqtt.Net.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin/Debug/M2Mqtt.Net

M2MQTT_NET_DLL_MDB_SOURCE=../bin/Debug/M2Mqtt.Net/M2Mqtt.Net.dll.mdb
M2MQTT_NET_DLL_MDB=$(BUILD_DIR)/M2Mqtt.Net.dll.mdb

endif

ifeq ($(CONFIG),RELEASE_X86)
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE;SSL"
ASSEMBLY = ../bin/Release/M2Mqtt.Net/M2Mqtt.Net.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../bin/Release/M2Mqtt.Net

M2MQTT_NET_DLL_MDB=

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(M2MQTT_NET_DLL_MDB)  


RESGEN=resgen2


FILES = \
	Exceptions/MqttClientException.cs \
	Exceptions/MqttCommunicationException.cs \
	Exceptions/MqttConnectionException.cs \
	Exceptions/MqttTimeoutException.cs \
	IMqttNetworkChannel.cs \
	Internal/InternalEvent.cs \
	Internal/MsgInternalEvent.cs \
	Internal/MsgPublishedInternalEvent.cs \
	Messages/MqttMsgBase.cs \
	Messages/MqttMsgConnack.cs \
	Messages/MqttMsgConnect.cs \
	Messages/MqttMsgConnectEventArgs.cs \
	Messages/MqttMsgContext.cs \
	Messages/MqttMsgDisconnect.cs \
	Messages/MqttMsgPingReq.cs \
	Messages/MqttMsgPingResp.cs \
	Messages/MqttMsgPuback.cs \
	Messages/MqttMsgPubcomp.cs \
	Messages/MqttMsgPublish.cs \
	Messages/MqttMsgPublishedEventArgs.cs \
	Messages/MqttMsgPublishEventArgs.cs \
	Messages/MqttMsgPubrec.cs \
	Messages/MqttMsgPubrel.cs \
	Messages/MqttMsgSuback.cs \
	Messages/MqttMsgSubscribe.cs \
	Messages/MqttMsgSubscribedEventArgs.cs \
	Messages/MqttMsgSubscribeEventArgs.cs \
	Messages/MqttMsgUnsuback.cs \
	Messages/MqttMsgUnsubscribe.cs \
	Messages/MqttMsgUnsubscribedEventArgs.cs \
	Messages/MqttMsgUnsubscribeEventArgs.cs \
	MqttClient.cs \
	MqttSecurity.cs \
	Net/Fx.cs \
	Net/MqttNetworkChannel.cs \
	MqttSettings.cs \
	Properties/AssemblyInfo.cs \
	Session/MqttBrokerSession.cs \
	Session/MqttClientSession.cs \
	Session/MqttSession.cs \
	Utility/Trace.cs \
	Utility/QueueExtension.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = 

REFERENCES =  \
	System \
	System.Core \
	System.Xml.Linq \
	System.Data.DataSetExtensions \
	Microsoft.CSharp \
	System.Data \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make





$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(M2MQTT_NET_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(M2MQTT_NET_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	make post-uninstall-local-hook prefix=$(prefix)
