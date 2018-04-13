<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" encoding="iso-8859-1"/>
  <xsl:strip-space elements="*" />

  <xsl:template match="/*">
    <xsl:for-each select="child::node()[1]">
      <xsl:for-each select="@*">"<xsl:value-of select="normalize-space(name())"/>"<xsl:if test="position() != last()">,</xsl:if>
      <xsl:if test="position() = last()"><xsl:text>&#xD;</xsl:text></xsl:if>
      </xsl:for-each>
    </xsl:for-each>

    <xsl:for-each select="child::*">
      <xsl:for-each select="@*">"<xsl:value-of select="normalize-space(.)" disable-output-escaping="yes"/>"<xsl:if test="position() != last()">,</xsl:if>
      <xsl:if test="position() = last()"><xsl:text>&#xD;</xsl:text></xsl:if>
      </xsl:for-each>
    </xsl:for-each>
    
  </xsl:template>
  
</xsl:stylesheet>
