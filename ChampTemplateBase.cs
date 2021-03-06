﻿using champ.Map;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xipton.Razor;

namespace champ
{
    public abstract class ChampTemplateBase : TemplateBase
    {
        public virtual PageListCollection Lists { get; private set; }

        public ChampTemplateBase()
        {
            Lists = new PageListCollection();
        }

        public virtual string ResolveUrl(string filename)
        {
            var node = Model.Page as PageNode;
            var prefix = "";
            var depth = node.GetDepth();
            for (var counter = 0; counter < depth; counter++)
            {
                prefix += "../";
            }
            return prefix + filename;
        }

        public virtual string ResolvePage(PageNode page)
        {
            return ResolveUrl(page.ToString());
        }

        /// <summary>
        /// Returns 'value' if the condition is true.
        /// </summary>
        public virtual string ValueIf(object condition, string value)
        {
            if (condition == null)
            {
                return String.Empty;
            }
            if (condition is String)
            {
                return (string)condition == String.Empty ? String.Empty : value;
            }
            if ((bool)condition)
            {
                return value;
            }
            else
            {
                return String.Empty;
            }
        }

        public virtual string ValueOrDefault(string value, string defaultValue)
        {
            return String.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public virtual string Include(string template, string key = null, string value = null)
        {
            var templateContent = SiteBuilder.Razor.GetTemplate("~/" + template + ".cshtml");
            BetterExpando viewbag;
            if(key != null && value != null)
            {
                viewbag = new BetterExpando();
                ((dynamic)viewbag)[key] = value;
                viewbag.Augment(this.ViewBag);
            }
            else
            {
                viewbag = this.ViewBag;
            }
            // Render the page at the template
            ITemplate output = SiteBuilder.Razor.Execute(templateContent, this.Model, viewbag: viewbag);
            return output.Result;
        }

        public virtual string EmbedFile(string filename)
        {
            var filePath = ResolveUrl(filename);
            return File.ReadAllText(filePath);
        }

        public virtual string Hash(string value)
        {
            return value.ToMD5Hash();
        }
    }

    public abstract class ChampTemplateBase<TModel> : ChampTemplateBase, ITemplate<TModel>
    {
        public new TModel Model
        {
            get { return GetOrCreateModel<TModel>(); }
        }
    }
}
