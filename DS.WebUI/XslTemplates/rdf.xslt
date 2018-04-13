<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#"
                xmlns:aws="http://xml.amazon.com/schemas3/">

  <!-- Convert XML to RDF that all describes one resource. Template
       rules after "End of template rules" comment are generic; those
       before are for customizing treatment of source XML
       (e.g. deleting elements). -->

  <!-- URL of the resource being described. -->
  <xsl:variable name="resourceURL">
    <xsl:value-of select="Root/Url"/>
  </xsl:variable>

  <!-- Namespace prefix for predicates. Needs a corresponding xmlns
       declaration in the xsl:stylesheet start-tag above. If your set
       of predicates come from more than one namespace, than this
       stylesheet is too simple for your needs. -->
  <xsl:variable name="nsPrefix">aws</xsl:variable>

  <!-- Elements to suppress. priority attribute necessary 
       because of template that adds rdf:parseType above. -->
  <xsl:template priority="1" match="Request|TotalResults|TotalPages"/>

  <!-- Just pass along contents without tags.  -->
  <xsl:template match="ProductInfo|Details">
    <xsl:apply-templates/>
  </xsl:template>

  <!-- ========================================================
       End of template rules addressing specific element types.
       Remaining template rules are generic xml2rdf template rules. 
       ======================================================== -->

  <xsl:template match="/">
    <rdf:RDF>
      <rdf:Description
       rdf:about="{$resourceURL}">
        <xsl:apply-templates/>
      </rdf:Description>
    </rdf:RDF>
  </xsl:template>

  <!-- Elements with URLs as content: convert them to store 
       their value in rdf:resource attribute of empty element -->
  <xsl:template match="*[starts-with(.,'http://') or starts-with(.,'urn:')]">
    <xsl:element name="{$nsPrefix}:{name()}">
      <xsl:attribute name="rdf:resource">
        <xsl:value-of select="."/>
      </xsl:attribute>
    </xsl:element>
  </xsl:template>

  <!-- Container elements: if the element has children and an element parent 
       (i.e. it isn't the root element) and it has no attributes, add
       rdf:parseType = "Resource". -->

  <xsl:template match="*[* and ../../* and not(@*)]">
    <xsl:element name="{$nsPrefix}:{name()}">
      <xsl:attribute name="rdf:parseType">Resource</xsl:attribute>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:element>
  </xsl:template>

  <!-- Copy remaining elements, putting them in a namespace. -->
  <xsl:template match="*">
    <xsl:if test="name() != 'Url'">
      <xsl:element name="{$nsPrefix}:{name()}">
        <xsl:apply-templates select="@*|node()"/>
      </xsl:element>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>
