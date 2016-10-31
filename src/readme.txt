Aegis V1.0.0

This application was created to improve the manual auditing process of network devices.  Network devices, such as switches and routers, are manually audited to ensure vulnerabilities are mitigated.  Our team was performing these functions on large enterprise networks.  We needed a tool to ensure the team was accurate, efficient, and most importantly comprehensive.  This tool provided a way to quickly scan network device configuration files and give feedback to ensure that the vulnerability identified within the DISA STIG was mitigated in some fashion.

Bastille V2.0.0

This update improved on the data imported within the tool.  Previous versions only allowed for a single key and value within a <dictionary>.  With regards to some of the DISA STIGs requiring multiple checks per vulnerability ID, we moved the application to an xml parse.  This allowed for multiple values to be stored under one key.  The dictionary could have worked with the use of tuple data being passed as the value to the key.  This would have been overcomplicated for future updates, so the xmlparser() method was added.  This stored all the vulnID information within list.
