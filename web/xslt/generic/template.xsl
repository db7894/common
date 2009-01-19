<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"
     doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"/> 

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Main Layout Template                               -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="/">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
  <head>
    <link rel="stylesheet" type="text/css" href="template.css"/>
  </head>
  <body>
<xsl:apply-templates/>
  </body>
</html>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Build Results Layout Template                      -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="buildresults">
  <div id="header">
    <h2>Build Results of
   	  <xsl:value-of select="@project" />
	</h2>
  </div>
  <xsl:apply-templates select="target"/>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Target Layout Template                             -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="target">
  <div class="target">
    <div class="thead">
      <h3><span>Target</span>
	    <xsl:value-of select="@name" />
      </h3>
	</div>
	<div class="tbody">
      <xsl:apply-templates select="task"/>
	</div>
	<div class="tfoot">
      <p>Total Time:
	    <xsl:value-of select="duration"/>
      </p>
	</div>
  </div>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Task Layout Template                               -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="task">
  <div class="task">
    <p class="task-name">
      <xsl:value-of select="message/@level"/> - 
	  <xsl:value-of select="@name" />
      <span>
	  (<xsl:value-of select="duration"/>) 
	  </span>
	</p>
    <div class="message">
      <xsl:apply-templates select="message"/>
    </div>
  </div>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Message Layout Template                            -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="message">
  <p>
    <xsl:value-of select="."/>
  </p>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Please use the style.css to work with styling      -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
</xsl:stylesheet>
