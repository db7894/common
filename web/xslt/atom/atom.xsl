<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
	xmlns:atom="http://www.w3.org/2005/Atom"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:gd="http://schemas.google.com/g/2005"
	xmlns:gr="http://www.google.com/schemas/reader/atom/">
<!--
	generic atom feed schema
	xmlns:atom="http://purl.org/atom/ns#"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:dc="http://purl.org/dc/elements/1.1/">
-->

<xsl:output method="html"/>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Root Match                                         -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="/">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
	<head>
		<link rel="stylesheet" type="text/css" href="atom.css"/>
	</head>
	<body>
		<xsl:apply-templates select="/atom:feed"/>
	</body>
</html>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Atom Feed Match                                    -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="/atom:feed">
	<div id="header">
		<h2><xsl:value-of select="atom:title"/></h2>
		<a href="{atom:link[@rel='self']/@href}">
			<xsl:value-of select="substring(atom:updated, 0, 11)"/>
		</a>
	</div>
	<div id="content">
		<xsl:apply-templates select="atom:entry"/>
	</div>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Atom Entry Match                                   -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="atom:entry">
	<div class="entry">
		<div class="ehead">
		<a href="{atom:link[@rel='alternate']/@href}" title="{substring(atom:published, 0, 11)}">
			<xsl:value-of select="atom:title"/>
		</a>
		</div>
		<div class="ebody">
		<xsl:choose>
			<xsl:when test="atom:content != ''">
				<p><xsl:value-of select="atom:content" disable-output-escaping="yes" /></p>
			</xsl:when>
			<xsl:otherwise>
				<p><xsl:value-of select="atom:summary" disable-output-escaping="yes" /></p>
			</xsl:otherwise>
		</xsl:choose>
		</div>
		<div class="efoot">
			<xsl:apply-templates select="atom:author"/>
		</div>
	</div>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Atom Entry Author Match                            -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<xsl:template match="atom:author">
	<xsl:choose>
	<xsl:when test="@gr:unknown-author='true'">
		<p>Author Unknown</p>
	</xsl:when>
	<xsl:otherwise>
		<p><xsl:value-of select="atom:name" /></p>
	</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
<!-- Please use the stylesheet to control formatting    -->
<!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
</xsl:stylesheet>

