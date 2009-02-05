#!/bin/bash
#---------------------------------------------------------------------------# 
# Globals
#---------------------------------------------------------------------------# 
# @note The version strings only apply to Net+OS 7.2 and may need to be
# modified for older/newer versions
#---------------------------------------------------------------------------# 
HOSTV6="2001:db8:1:1:218:71ff:fe77:32ac"
HOSTV4="172.16.102.105"
SNMP="1.3.6.1.4.1.232.2.2.4.2.0"

#---------------------------------------------------------------------------# 
# @brief Protocal Response Strings
#---------------------------------------------------------------------------# 
HTTP_RSP="Server: Allegro-Software-RomPager/4.01"
FTP_RSP="220 NET+OS 7.2 FTP server ready."
TELNET_RSP="HP UPS Management Module"

#---------------------------------------------------------------------------# 
# @briev Quick Helper Macros
#---------------------------------------------------------------------------# 
SEARCH=" 2>/dev/null | grep"
GET4="nc4 --idle-timeout=1 -4 ${HOSTV4}"
GET6="nc6 --idle-timeout=1 -6 ${HOSTV6}"

#---------------------------------------------------------------------------# 
# @brief Tool Test
#---------------------------------------------------------------------------# 
if [ ! `which nc6` ]; then
	echo "Please Install netcat6 to continue..."
	exit -1
fi

if [ ! `which ping` ]; then
	echo "Please Install ping to continue..."
	exit -1
fi

if [ ! `which ping6` ]; then
	echo "Please Install ping6 to continue..."
	exit -1
fi

if [ ! `which snmpget` ]; then
	echo "Please Install net-snmp tools(snmpget) to continue..."
	exit -1
fi

#---------------------------------------------------------------------------# 
# @brief Test ipv4/6 snmp traps
#---------------------------------------------------------------------------# 
# Contents of /etc/snmp/snmptrapd.conf:
# authCommunity log,net public
# authCommunity log,net private
#---------------------------------------------------------------------------# 
test_traps()
{
	sudo snmptrapd -Lo -n -f -m ALL -Os udp6:162 udp:162
}

#---------------------------------------------------------------------------# 
# @brief Test ipv6 enabled services
#---------------------------------------------------------------------------# 
# This script will print test status only if a test fails.
#---------------------------------------------------------------------------# 
test_ipv6()
{
	/bin/ping6 -c1 ${HOSTV6} > /dev/null
	if [ "_$?" != "_0" ]; then
		echo "IPV6 Ping Failed"
	fi

	snmpget -v1 -c public udp6:[${HOSTV6}] ${SNMP} > /dev/null
	if [ "_$?" != "_0" ]; then
		echo "IPV6 SNMP Failed"
	fi

	a="`echo -e "GET / HTTP / 1.1\n" | ${GET6} 80 ${SEARCH} "${HTTP_RSP}"`"
	[ ! "$a" ] && echo "IPV6 HTTP Failed"

	a="`${GET6} 21 ${SEARCH} "${FTP_RSP}"`"
	[ ! "$a" ] && echo "IPV6 FTP Failed"

	a="`echo | ${GET6} 23 ${SEARCH} "${TELNET_RSP}"`"
	[ ! "$a" ] && echo "IPV6 Telnet Failed"

	unset a
}

#---------------------------------------------------------------------------# 
# @brief Test ipv4 enabled services
#---------------------------------------------------------------------------# 
# This script will print test status only if a test fails.
#---------------------------------------------------------------------------# 
test_ipv4()
{
	/bin/ping -c1 ${HOSTV4} > /dev/null
	if [ "_$?" != "_0" ]; then
		echo "IPV4 Ping Failed"
	fi

	snmpget -v1 -c public ${HOSTV4} ${SNMP} > /dev/null
	if [ "_$?" != "_0" ]; then
		echo "IPV4 SNMP Failed"
	fi

	a="`echo -e "GET / HTTP / 1.1\n" | ${GET4} 80 ${SEARCH} "${HTTP_RSP}"`"
	[ ! "$a" ] && echo "IPV4 HTTP Failed"

	a="`${GET4} 21 ${SEARCH} "${FTP_RSP}"`"
	[ ! "$a" ] && echo "IPV4 FTP Failed"

	a="`echo | ${GET4} 23 ${SEARCH} "${TELNET_RSP}"`"
	[ ! "$a" ] && echo "IPV4 Telnet Failed"

	unset a
}

#---------------------------------------------------------------------------# 
# Main
#---------------------------------------------------------------------------# 
case "$1" in
	ipv4) test_ipv4		;;
	ipv6) test_ipv6		;;
	both) test_ipv4
		  test_ipv6		;;
	traps) test_traps	;;
	*)
		echo "$0 (ipv4|ipv6|both|traps)"
esac
