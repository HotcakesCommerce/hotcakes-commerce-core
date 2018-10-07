<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html"/>
  <xsl:param name="ControlID" />
  <xsl:param name="Options" />
  <xsl:template match="/*">
    <xsl:apply-templates select="root" />
  </xsl:template>
  <xsl:template match="root">
    <xsl:if test="node">
      <ul class="filetree" id="{$ControlID}">
        <xsl:apply-templates select="node" />
      </ul>
      <script type="text/javascript">
        jQuery(function($) {
          $("#<xsl:value-of select="$ControlID" />").treeview(
            <xsl:value-of select="$Options" disable-output-escaping="yes" />
          );
        });
      </script>
    </xsl:if>
  </xsl:template>
  <xsl:template match="node">
    <li>
      <xsl:if test="@breadcrumb=0 or @selected=1">
        <xsl:attribute name="class">closed</xsl:attribute>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="@enabled = 0">
          <a><xsl:value-of select="@text" /></a>
        </xsl:when>
        <xsl:otherwise>
          <a href="{@url}">
            <xsl:choose>
              <xsl:when test="@selected=1">
                <xsl:attribute name="class">selected</xsl:attribute>
              </xsl:when>
              <xsl:when test="@breadcrumb=1">
                <xsl:attribute name="class">current</xsl:attribute>
              </xsl:when>
            </xsl:choose>
            <xsl:value-of select="@text" />
          </a>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:if test="node">
        <ul style="list-item-style:none">
          <xsl:apply-templates select="node" />
        </ul>
      </xsl:if>
    </li>
  </xsl:template>
</xsl:stylesheet>
