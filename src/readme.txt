Aegis V1.0.0

This application was created to improve the manual auditing process of network devices.  Network devices, such as switches and routers, are manually audited to ensure vulnerabilities are mitigated.  Our team was performing these functions on large enterprise networks.  We needed a tool to ensure the team was accurate, efficient, and most importantly comprehensive.  This tool provided a way to quickly scan network device configuration files and give feedback to ensure that the vulnerability identified within the DISA STIG was mitigated in some fashion.

Bastille V2.0.0

This update improved on the data imported within the tool.  Previous versions only allowed for a single key and value within a <dictionary>.  With regards to some of the DISA STIGs requiring multiple checks per vulnerability ID, we moved the application to an xml parse.  This allowed for multiple values to be stored under one key.  The dictionary could have worked with the use of tuple data being passed as the value to the key.  This would have been overcomplicated for future updates, so the xmlparser() method was added.  This stored all the vulnID information within list.

Citadel V3.0.0

NMAT Improvements:

  Updated: V-3008 Third Check "Crypto DynamicPOL"
  Updated: V-3043 First Check "SNMP  Community"
  Updated: V-3196 First Check "SNMP CONTrol" Second Check "SNMP Mib2TrapControl"
  Updated: V-3969 First Check "SNMP  Community"
  Updated: V-14681 First Check "BGP"
  Updated: V-14693 First Check look for "FEC" "FED" "FEE" "FEF"
  Updated: V-14707 First Check "RPF"
  Updated: V-15288 First Check "ISATAP"
  Updated: V-17815 First Check "OSPF"
  Updated: V-17816 First Check "OSPF"
  Updated: V-19189 First Check "-MIP" Second Check "-PIM Control" Third Check "-PIM StaticRP"

User Interface:

  Updated: When user imports new file, VulnID dropdown list needs to reset to top selection
  Updated: When user types in search window, clear VulnID and remove multiple steps groupbox
  Updated: Add "Next" and "Previous" buttons to go through the VulnID list quicker
  Updated: Router Device IP is not populating properly.  SystemIP works to find the correct IP address
  Updated: If "none" is in the field, then multiple choice step doens't need to show up.
