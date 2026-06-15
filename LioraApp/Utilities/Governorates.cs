using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LioraApp.Utilities;

public static class Governorates
{
    public static IEnumerable<SelectListItem> GetEgyptianGovernorates()
    {
        return new List<SelectListItem>
        {
            new SelectListItem("القاهرة", "القاهر") ,
            new SelectListItem("الجيزة", "الجيزة"),
            new SelectListItem("الإسكندرية", "الإسكندرية"),
            new SelectListItem("القليوبية", "القليوبية"),
            new SelectListItem("المنوفية", "المنوفية"),
            new SelectListItem("الغربية", "الغربية"),
            new SelectListItem("الشرقية", "الشرقية"),
            new SelectListItem("الدقهلية", "الدقهلية"),
            new SelectListItem("البحيرة", "البحيرة"),
            new SelectListItem("كفر الشيخ", "كفر الشيخ"),
            new SelectListItem("دمياط", "دمياط"),
            new SelectListItem("بورسعيد", "بورسعيد"),
            new SelectListItem("الإسماعيلية", "الإسماعيلية"),
            new SelectListItem("السويس", "السويس"),
            new SelectListItem("الفيوم", "الفيوم"),
            new SelectListItem("بني سويف", "بني سويف"),
            new SelectListItem("المنيا", "المنيا"),
            new SelectListItem("أسيوط", "أسيوط"),
            new SelectListItem("سوهاج", "سوهاج"),
            new SelectListItem("قنا", "قنا"),
            new SelectListItem("الأقصر", "الأقصر"),
            new SelectListItem("أسوان", "أسوان"),
            new SelectListItem("البحر الأحمر", "البحر الأحمر"),
            new SelectListItem("الوادى الجديد", "الوادى الجديد"),
            new SelectListItem("مطروح", "مطروح"),
            new SelectListItem("شمال سيناء", "شمال سيناء"),
            new SelectListItem("جنوب سيناء", "جنوب سيناء")
        };
    }
}