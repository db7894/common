#!/bin/bash
ENVROOT=`cd $(/usr/bin/dirname $0) 2>/dev/null && cd .. && pwd;`;
mkdir -p ${ENVROOT}/var/tmp 2>&1 > /dev/null;
[ -z "$TZ" ] && TZ=UTC; export TZ;
JAVA_HOME=${JAVA_HOME-$(ls -1vrd $ENVROOT/jdk1.* | head -n1)}; export JAVA_HOME;
CLASSPATH="$ENVROOT/Scala2.10/*:$ENVROOT/lib/*:$ENVROOT/akka-config"
exec ${JAVA_HOME}/bin/java \
  "-Xms4096M" \
  "-Xmx4096M" \
  -Xshare:off \
  -XX:MaxPermSize=512m \
  -XX:PermSize=512m \
  -XX:HeapDumpPath=/tmp \
  -server \
  -XX:+HeapDumpOnOutOfMemoryError \
  -XX:+UseParallelGC \
  -Dcom.sun.management.jmxremote \
  -Dcom.sun.management.jmxremote.port=11400 \
  -Dcom.sun.management.jmxremote.ssl=false \
  -Dcom.sun.management.jmxremote.authenticate=false \
  -Dpid=$$ \
  -XX:OnOutOfMemoryError="/bin/kill -9 %p" \
  -Dsun.net.inetaddr.ttl=0 \
  -Dnetworkaddress.cache.ttl=0 \
  -Djava.io.tmpdir=${ENVROOT}/var/output/tmp \
  -classpath "${CLASSPATH}" \
  -Djava.library.path=${ENVROOT}/lib \
  -Dapollo.environment.root=${ENVROOT} \
  ${JVM_ARGS} \
  com.bashwork.application.Main \
  -- \
  "$@" \
  ;
