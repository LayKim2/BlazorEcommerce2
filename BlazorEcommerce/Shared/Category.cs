using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Url { get; set; } = String.Empty;

        public bool Visible { get; set; } = true;
        public bool Deleted { get; set; } = false;

        // TODO -- Model엔 있지만, table의 column에 추가하고 싶지 않을 때,
        [NotMapped]
        public bool Editing { get; set; } = false;
        [NotMapped]
        public bool IsNew { get; set; } = false;
    }
}
