using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Diagnostics;

public class ItemInfo
{
    public string label;
    public string type;
    public string rule;
    public string enums;

    public ItemInfo(string l, string t, string r, string e)
    {
        label = l;
        type = t;
        rule = r;
        enums = e;
    }
}

public class InterviewItem
{
    public static DataTable getInterviewItems()
    {
        InterviewItem i = new InterviewItem();
        DataTable t = new DataTable();
        t.Clear();
        t.Columns.Add("INTV_NAME");
        t.Columns.Add("INTV_CD");

        List<string> list = new List<string>(i.items.Keys);

        DataRow r;
        foreach (string k in list)
        {
            r = t.NewRow();
            r["INTV_NAME"] = i.items[k];
            r["INTV_CD"] = k;
            t.Rows.Add(r);
        }

        return t;
    }

    private Dictionary<string, string> items;
    private Dictionary<string, Dictionary<string, ItemInfo>> iv_items;
    private Dictionary<string, Dictionary<string, string>> iv_enum;

    public Dictionary<string, string> getDictionary() {
        return items;
    }

    public InterviewItem()
    {
        items = new Dictionary<string, string>();

        items["R.RSVNO"] = "예약번호";
        items["R.RSVDATE"] = "수진일";
        items["P.PATNAME"] = "성명";
        items["P.ENGNAME"] = "영문성명";
        items["P.PT_NO"] = "환자번호";
        items["P.DATE_OF_BIRTH"] = "생년월일";
        items["P.AGE"] = "환자나이";
        items["P.SEX"] = "성별";
        items["R.PKGCODE"] = "패키지코드";
        items["I3.HTN"] = "고혈압";
        items["I3.CAD"] = "협심증/심근경색";
        items["I3.revas"] = "재관류술";
        items["I3.dyslipid"] = "고지혈증";
        items["I3.DM"] = "당뇨병";
        items["I3.osteo"] = "골다공증";
        items["I3.asthma"] = "기관지천식";
        items["I3.all_rhin"] = "알러지비염";
        items["I3.ulcer"] = "위/십이지장궤양";
        items["I3.refl_eso"] = "역류성 식도염";
        items["I3.CRD"] = "만성신장질환";
        items["I3.BPH"] = "전립선 비대증";
        items["I3.stroke"] = "뇌졸중/중풍";
        items["I3.col_polyp"] = "대장용종";
        items["I3.gallstone"] = "담석";
        items["I3.gb_polyp"] = "담낭용종";
        items["I3.hemo"] = "치핵, 치루";
        items["I3.uri_stone"] = "신장결석";
        items["I3.lung_ca"] = "폐암";
        items["I3.stoma_ca"] = "위암";
        items["I3.col_ca"] = "대장암";
        items["I3.hcc"] = "간암";
        items["I3.breast_ca"] = "유방암";
        items["I3.cervix_ca"] = "자궁경부암";
        items["I3.thyroid_ca"] = "갑상선암";
        items["I3.surg_abd"] = "개복수술";
        items["I3.surg_other"] = "기타수술";
        items["I3.other_ds"] = "기타질환";
        items["I3.aspirin"] = "아스피린 복용";
        items["I3.anti_coa"] = "항응고제 복용";
        items["I3.cal_suppl"] = "칼슘제 복용";
        items["I3.nsaid"] = "소염진통제 복용";
        items["I3.steroid"] = "스테로이드제 복용";
        items["I3.estrogen"] = "여성호르몬제 복용";
        items["I3.consistent"] = "지속적복용약없음";
        items["I3.contrast"] = "조영제부작용";
        items["I3.allergy"] = "알레르기";
        items["I3.helico"] = "헬리코박터 제균";
        items["I3.hp_cause"] = "제균이유";
        items["I3.hp_when"] = "제균시기";
        items["I3.smoking"] = "흡연";
        items["I3.smk_py"] = "흡연량(pack-year)";
        items["I3.dairy"] = "유제품 섭취";
        items["I3.salty"] = "음식간을 추가로 한다";
        items["I3.alcohol_yes"] = "ver3_현재음주여부";
        items["I3.alcohol"] = "ver3/4_주당 음주량";
        items["I3.alc_frequency"] = "ver2/4_음주 빈도";
        items["I3.alc_amount"] = "ver2/4_한번음주량";
        items["I3.education"] = "학력";
        items["I3.income"] = "수입";
        items["I3.Menarche"] = "초경";
        items["I3.menarche_school"] = "ver3_초경학년";
        items["I3.parity"] = "분만력";
        items["I3.menopause"] = "폐경여부";
        items["I3.memo_surg"] = "외과적 폐경";
        items["I3.HRT"] = "호르몬치료";
        items["I3.HRT_dur"] = "호르몬치료기간";
        items["I3.HRT_ex_dur"] = "중단후 기간";
        items["I3.HRT_drug"] = "호르몬 종류";
        items["I3.GI_1"] = "음식을삼킬때 목이나 가슴부위에 음식이 걸린듯하다";
        items["I3.GI_2"] = "구역질이나 구토가 자주난다";
        items["I3.GI_3"] = "소화가 안되고, 헛배가 부른다";
        items["I3.GI_4"] = "ver2_위에 음식물이 남아 있는 듯한 느낌이 지속된다";
        items["I3.GI_5"] = "공복시에나 식후에 속이 쓰리다";
        items["I3.GI_6"] = "신물이 자주 넘어오고 가슴쪽에 화끈한증상이 있다";
        items["I3.GI_7"] = "ver2_헛배부르고 배에 가스가 찬다";
        items["I3.GI_8"] = "대변색이 자장처럼 검게 나온적이 있다";
        items["I3.GI_9"] = "대변에 붉은 피가 섞여 나온적이 있다";
        items["I3.GI_10"] = "최근에 배변습관이 현저하게 바뀌었다";
        items["I3.GI_11"] = "ver2_최근에 설사가 자주난다";
        items["I3.GI_12"] = "ver2_최근에 변비로 고생한다";
        items["I3.GI_13"] = "ver2_최근들어 변이 가늘게 나온다";
        items["I3.GI_14"] = "ver2_배에 덩어리가 만져 진다";
        items["I3.wt_loss"] = "체중감소 여부";
        items["I3.wt_loss_kg"] = "체중감소 정도";
        items["I3.wt_loss_du"] = "체중감소 기간";
        items["I3.FH_HTN"] = "고혈압 가족력";
        items["I3.FH_CAD"] = "심근경색/협심증 가족력";
        items["I3.FH_DM"] = "당뇨 가족력";
        items["I3.FM_stroke"] = "뇌졸중 가족력";
        items["I3.FM_stom_ca1"] = "위암 가족력1";
        items["I3.FM_stom_ca2"] = "위암 가족력2";
        items["I3.FM_col_ca1"] = "대장/직장암 가족력1";
        items["I3.FM_col_ca2"] = "대장/직장암 가족력2";
        items["I3.IPSS_total"] = "IPSS 총점";   // 여성은 제외...
        items["I3.IPSS_inconv"] = "IPSS 불편감";   // 여성은 제외...
        items["I3.BDI_total"] = "BDI 총점";
        items["I3.HTN_dx"] = "ver4_고혈압 진단력";
        items["I3.CAD_dx"] = "ver4_협심증/심근경색 진단력";
        items["I3.dyslipid_dx"] = "ver4_고지혈증 진단력";
        items["I3.DM_dx"] = "ver4_당뇨병 진단력";
        items["I3.asthma_dx"] = "ver4_기관지천식 진단력";
        items["I3.all_rhin_dx"] = "ver4_알러지비염 진단력";
        items["I3.CRD_dx"] = "ver4_만성신장질환 진단력";
        items["I3.stroke_dx"] = "ver4_뇌졸중/중풍 진단력";
        items["I3.cirrhosis"] = "ver4_간경변";
        items["I3.cirrhosis_dx"] = "ver4_간경변 진단력";
        items["I3.HBV"] = "ver4_만성 B형 간염";
        items["I3.HBV_dx"] = "ver4_만성 B형 간염 진단력";
        items["I3.HCV"] = "ver4_만성 C형 간염";
        items["I3.HCV_dx"] = "ver4_만성 C형 간염 진단력";
        items["I3.TB"] = "ver4_폐결핵";
        items["I3.TB_dx"] = "ver4_폐결핵 진단력";
        items["I3.pastTB"] = "ver4_폐결핵치료력";
        items["I3.Tbscar"] = "ver4_폐결핵흔적";
        items["I3.fracture"] = "ver4_골절";
        items["I3.other_ds_dx"] = "ver4_기타질환명";
        items["I3.prostate_ca"] = "ver4_전립선암";
        items["I3.other_ca"] = "ver4_기타암";
        items["I3.surg_abd_dx"] = "ver4_개복수술 진단명";
        items["I3.surg_other_ds"] = "ver4_기타 수술 진단명";
        items["I3.thyroid"] = "ver4_갑상선호르몬";
        items["I3.anti_thyroid"] = "ver4_갑상선기능항진증약";
        items["I3.osteoporosis"] = "ver4_골다공증약";
        items["I3.sedative"] = "ver4_진정제,수면제";
        items["I3.al_cause"] = "ver4_알레르기의 원인";
        items["I3.hp_eradi"] = "ver4_헬리코박터 제균 여부";
        items["I3.meal_freq"] = "ver4_매끼 식사 여부";
        items["I3.meal_amount"] = "ver4_식사량";
        items["I3.meal_out"] = "ver4_외식은 얼마나 자주";
        items["I3.meal_snack"] = "ver4_간식";
        items["I3.meal_rice"] = "ver4_한 끼 밥량";
        items["I3.meal_carbo"] = "ver4_곡류 섭취";
        items["I3.meal_prot"] = "ver4_단백질 섭취";
        items["I3.meal_veget"] = "ver4_채소 반찬";
        items["I3.meal_dairy"] = "ver4_유제품 섭취";
        items["I3.meal_fruit"] = "ver4_과일 섭취";
        items["I3.meal_water"] = "ver4_물 섭취";
        items["I3.meal_fried"] = "ver4_튀김 부침개 섭취";
        items["I3.meal_fatty"] = "ver4_지방 육류";
        items["I3.meal_instant"] = "ver4_인스턴트 가공 식품";
        items["I3.meal_salty1"] = "ver4_음식간을 추가로 한다";
        items["I3.meal_salty2"] = "ver4_젓갈 장아찌";
        items["I3.meal_sugar"] = "ver4_단 음식";
        items["I3.meal_juice"] = "ver4_주스, 음료, 드링크류";
        items["I3.meal_coffee"] = "ver4_커피";
        items["I3.exer_heavy"] = "ver4_격렬한 활동 일수";
        items["I3.exer_heavy_dur"] = "ver4_격렬한 활동 시간";
        items["I3.exer_mod"] = "ver4_중간정도 활동 일수";
        items["I3.exer_mod_dur"] = "ver4_중간정도 활동 시간";
        items["I3.exer_light"] = "ver4_10분이상 걸은 날수";
        items["I3.exer_light_dur"] = "ver4_걸은 시간";
        items["I3.exercise"] = "ver4_주당 운동시간";
        items["I3.exer_MET"] = "ver4_주당 활동량 MET";
        items["I3.FM_cirrhosis"] = "ver4_만성간염, 간경변 가족력";
        items["I3.FM_liver_ca"] = "ver4_간암 가족력";
        items["I3.FM_lung_ca"] = "ver4_폐암 가족력";

        iv_items = new Dictionary<string, Dictionary<string, ItemInfo>>();

        //--------------------------------------------------------------------------------------------
        // 2차 문진표 항목 ---------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------

        iv_items["2nd"] = new Dictionary<string, ItemInfo>();
        iv_items["2nd"]["HTN"] = new ItemInfo("고혈압", "OR_BOOL", "3::A004", "BOOL_BASE");
        iv_items["2nd"]["CAD"] = new ItemInfo("협심증/심근경색", "OR_BOOL", "3::A012", "BOOL_BASE");
        iv_items["2nd"]["dyslipid"] = new ItemInfo("고지혈증", "OR_BOOL", "5::A030", "BOOL_BASE");
        iv_items["2nd"]["DM"] = new ItemInfo("당뇨병", "OR_BOOL", "3::A009", "BOOL_BASE");
        iv_items["2nd"]["osteo"] = new ItemInfo("골다공증", "OR_BOOL", "5::A032", "BOOL_BASE");
        iv_items["2nd"]["asthma"] = new ItemInfo("기관지천식", "OR_BOOL", "3::A013", "BOOL_BASE");
        iv_items["2nd"]["CRD"] = new ItemInfo("만성신장질환", "OR_BOOL", "3::A016", "BOOL_BASE");
        iv_items["2nd"]["BPH"] = new ItemInfo("전립선 비대증", "OR_BOOL", "3::A014", "BOOL_BASE");
        iv_items["2nd"]["other_ds"] = new ItemInfo("기타질환", "TEXT", "4::A019", "");
        iv_items["2nd"]["aspirin"] = new ItemInfo("아스피린 복용", "OR_BOOL", "5::A028", "BOOL_BASE");
        iv_items["2nd"]["anti_coa"] = new ItemInfo("항응고제 복용", "OR_BOOL", "3::A019", "BOOL_BASE");
        iv_items["2nd"]["cal_suppl"] = new ItemInfo("칼슘제 복용", "OR_BOOL", "5::A032", "BOOL_BASE");
        iv_items["2nd"]["nsaid"] = new ItemInfo("소염진통제 복용", "OR_BOOL", "5::A034", "BOOL_BASE");
        iv_items["2nd"]["estrogen"] = new ItemInfo("여성호르몬제 복용", "OR_BOOL", "5::A031", "BOOL_BASE");
        iv_items["2nd"]["contrast"] = new ItemInfo("조영제부작용", "OR_BOOL", "3::A021", "BOOL_BASE");
        iv_items["2nd"]["helico"] = new ItemInfo("헬리코박터 제균", "MULTI", "3::A023, 2 || 3::A024, 0 || 3::A025, 1", "MULTI_KNOW");
        iv_items["2nd"]["smoking"] = new ItemInfo("흡연", "MULTI", "6::A069, 0 || 6::A070, 1 || 6::A077, 2", "MULTI_smoking");
        iv_items["2nd"]["smk_py"] = new ItemInfo("흡연량(pack-year)", "CALC", "FUNC:smk_py_2nd", "");
        iv_items["2nd"]["dairy"] = new ItemInfo("유제품 섭취", "OR_BOOL", "11::A010", "BOOL_BASE");
        iv_items["2nd"]["salty"] = new ItemInfo("음식간을 추가로 한다", "OR_BOOL", "12::A015", "BOOL_BASE");
        iv_items["2nd"]["alc_frequency"] = new ItemInfo("음주 빈도", "MULTI", "6::A084, 1 || 6::A085, 2 || 6::A086, 3 || 6::A087, 4 || 6::A088, 5 || 6::A089, 6", "MULTI_VALUE");
        iv_items["2nd"]["alc_amount"] = new ItemInfo("한번음주량", "MULTI", "6::A091, 1 || 6::A092, 2 || 6::A093, 3 || 6::A094, 4", "MULTI_VALUE");
        iv_items["2nd"]["education"] = new ItemInfo("학력", "MULTI", "7::A042, 1 || 7::A043, 2 || 7::A044, 3 || 7::A045, 4 || 7::A046, 5", "MULTI_education");
        iv_items["2nd"]["income"] = new ItemInfo("수입", "MULTI", "7::A047, 1 || 7::A048, 2 || 7::A049, 3 || 7::A050, 4 || 7::A051, 5 || 7::A052, 6 || 7::A053, 7 || 7::A054, 8", "MULTI_income");
        iv_items["2nd"]["Menarche"] = new ItemInfo("초경", "TEXT", "21::A002", "");
        iv_items["2nd"]["parity"] = new ItemInfo("분만력", "OR_BOOL", "21::A005", "BOOL_BASE");
        iv_items["2nd"]["menopause"] = new ItemInfo("폐경여부", "MULTI", "21::A041, 0 || 21::A042, 1", "MULTI_menopause");
        iv_items["2nd"]["memo_surg"] = new ItemInfo("외과적 폐경", "CALC", "FUNC:memo_surg_2nd", "");
        iv_items["2nd"]["GI_1"] = new ItemInfo("음식을삼킬때 목이나 가슴부위에 음식이 걸린듯하다", "OR_BOOL", "8::A009", "BOOL_BASE");
        iv_items["2nd"]["GI_2"] = new ItemInfo("구역질이나 구토가 자주난다", "OR_BOOL", "8::A010", "BOOL_BASE");
        iv_items["2nd"]["GI_3"] = new ItemInfo("소화가 안되고, 헛배가 부른다", "OR_BOOL", "8::A011", "BOOL_BASE");
        iv_items["2nd"]["GI_4"] = new ItemInfo("위에 음식물이 남아 있는 듯한 느낌이 지속된다", "OR_BOOL", "8::A012", "BOOL_BASE");
        iv_items["2nd"]["GI_5"] = new ItemInfo("공복시에나 식후에 속이 쓰리다", "OR_BOOL", "8::A013", "BOOL_BASE");
        iv_items["2nd"]["GI_6"] = new ItemInfo("신물이 자주 넘어오고 가슴쪽에 화끈한증상이 있다", "OR_BOOL", "8::A014", "BOOL_BASE");
        iv_items["2nd"]["GI_7"] = new ItemInfo("헛배부르고 배에 가스가 찬다", "OR_BOOL", "8::A015", "BOOL_BASE");
        iv_items["2nd"]["GI_8"] = new ItemInfo("대변색이 자장처럼 검게 나온적이 있다", "OR_BOOL", "8::A016", "BOOL_BASE");
        iv_items["2nd"]["GI_9"] = new ItemInfo("대변에 붉은 피가 섞여 나온적이 있다", "OR_BOOL", "8::A017", "BOOL_BASE");
        iv_items["2nd"]["GI_11"] = new ItemInfo("최근에 설사가 자주난다", "OR_BOOL", "8::A018", "BOOL_BASE");
        iv_items["2nd"]["GI_12"] = new ItemInfo("최근에 변비로 고생한다", "OR_BOOL", "8::A019", "BOOL_BASE");
        iv_items["2nd"]["GI_13"] = new ItemInfo("최근들어 변이 가늘게 나온다", "OR_BOOL", "8::A020", "BOOL_BASE");
        iv_items["2nd"]["GI_14"] = new ItemInfo("배에 덩어리가 만져 진다", "OR_BOOL", "8::A021", "BOOL_BASE");
        iv_items["2nd"]["FH_HTN"] = new ItemInfo("고혈압 가족력", "OR_BOOL", "6::A005 || 6::A006 || 6::A007 || 6::A008", "BOOL_FH_1st");
        iv_items["2nd"]["FH_CAD"] = new ItemInfo("심근경색/협심증 가족력", "OR_BOOL", "8::A021 || 8::A022 || 8::A023 || 8::A024", "BOOL_FH_1st");
        iv_items["2nd"]["FH_DM"] = new ItemInfo("당뇨 가족력", "OR_BOOL", "8::A029 || 8::A030 || 8::A031 || 8::A032", "BOOL_FH_1st");
        iv_items["2nd"]["FM_stroke"] = new ItemInfo("뇌졸중 가족력", "OR_BOOL", "8::A029 || 8::A030 || 8::A031 || 8::A032", "BOOL_FH_1st");
        iv_items["2nd"]["IPSS_total"] = new ItemInfo("IPSS 총점", "CALC", "FUNC:ipss_total_2nd", "");
        iv_items["2nd"]["IPSS_inconv"] = new ItemInfo("IPSS 불편감", "TEXT", "16::A008", "");

        //--------------------------------------------------------------------------------------------
        // 3차 문진표 항목 ---------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------

        iv_items["3rd"] = new Dictionary<string, ItemInfo>();
        iv_items["3rd"]["HTN"] = new ItemInfo("고혈압", "OR_BOOL", "4::A001 || 4::A037", "BOOL_BASE");
        iv_items["3rd"]["CAD"] = new ItemInfo("협심증/심근경색", "OR_BOOL", "4::A002 || 4::A039", "BOOL_BASE");
        iv_items["3rd"]["revas"] = new ItemInfo("재관류술", "OR_BOOL", "4::A092 || 4::A093", "BOOL_BASE");
        iv_items["3rd"]["dyslipid"] = new ItemInfo("고지혈증", "OR_BOOL", "4::A003 || 4::A041", "BOOL_BASE");
        iv_items["3rd"]["DM"] = new ItemInfo("당뇨병", "OR_BOOL", "4::A004 || 4::A043", "BOOL_BASE");
        iv_items["3rd"]["osteo"] = new ItemInfo("골다공증", "OR_BOOL", "4::A007 || 4::A049", "BOOL_BASE");
        iv_items["3rd"]["asthma"] = new ItemInfo("기관지천식", "OR_BOOL", "4::A009 || 4::A053", "BOOL_BASE");
        iv_items["3rd"]["all_rhin"] = new ItemInfo("알러지비염", "OR_BOOL", "4::A010 || 4::A055", "BOOL_BASE");
        iv_items["3rd"]["ulcer"] = new ItemInfo("위/십이지장궤양", "OR_BOOL", "4::A011 || 4::A057", "BOOL_BASE");
        iv_items["3rd"]["refl_eso"] = new ItemInfo("역류성 식도염", "OR_BOOL", "4::A012 || 4::A059", "BOOL_BASE");
        iv_items["3rd"]["CRD"] = new ItemInfo("만성신장질환", "OR_BOOL", "4::A013 || 4::A061", "BOOL_BASE");
        iv_items["3rd"]["BPH"] = new ItemInfo("전립선 비대증", "OR_BOOL", "4::A014 || 4::A063", "BOOL_BASE");
        iv_items["3rd"]["stroke"] = new ItemInfo("뇌졸중/중풍", "OR_BOOL", "4::A017 || 4::A069", "BOOL_BASE");
        iv_items["3rd"]["col_polyp"] = new ItemInfo("대장용종", "OR_BOOL", "5::A001 || 5::A040", "BOOL_BASE");
        iv_items["3rd"]["gallstone"] = new ItemInfo("담석", "OR_BOOL", "5::A004 || 5::A046", "BOOL_BASE");
        iv_items["3rd"]["gb_polyp"] = new ItemInfo("담낭용종", "OR_BOOL", "5::A005 || 5::A048", "BOOL_BASE");
        iv_items["3rd"]["hemo"] = new ItemInfo("치핵, 치루", "OR_BOOL", "5::A006 || 5::A050", "BOOL_BASE");
        iv_items["3rd"]["uri_stone"] = new ItemInfo("신장결석", "OR_BOOL", "5::A009 || 5::A056", "BOOL_BASE");
        iv_items["3rd"]["lung_ca"] = new ItemInfo("폐암", "OR_BOOL", "5::A069", "BOOL_BASE");
        iv_items["3rd"]["stoma_ca"] = new ItemInfo("위암", "OR_BOOL", "5::A070", "BOOL_BASE");
        iv_items["3rd"]["col_ca"] = new ItemInfo("대장암", "OR_BOOL", "5::A071", "BOOL_BASE");
        iv_items["3rd"]["hcc"] = new ItemInfo("간암", "OR_BOOL", "5::A072", "BOOL_BASE");
        iv_items["3rd"]["breast_ca"] = new ItemInfo("유방암", "OR_BOOL", "5::A073", "BOOL_BASE");
        iv_items["3rd"]["cervix_ca"] = new ItemInfo("자궁경부암", "OR_BOOL", "5::A074", "BOOL_BASE");
        iv_items["3rd"]["thyroid_ca"] = new ItemInfo("갑상선암", "OR_BOOL", "5::A075", "BOOL_BASE");
        iv_items["3rd"]["surg_abd"] = new ItemInfo("개복수술", "TEXT", "5::A062", "");
        iv_items["3rd"]["surg_other"] = new ItemInfo("기타수술", "TEXT", "5::A063", "");
        iv_items["3rd"]["other_ds"] = new ItemInfo("기타질환", "TEXT", "5::A078", "");
        iv_items["3rd"]["aspirin"] = new ItemInfo("아스피린 복용", "OR_BOOL", "6::A001", "BOOL_BASE");
        iv_items["3rd"]["anti_coa"] = new ItemInfo("항응고제 복용", "OR_BOOL", "6::A002", "BOOL_BASE");
        iv_items["3rd"]["cal_suppl"] = new ItemInfo("칼슘제 복용", "OR_BOOL", "6::A003", "BOOL_BASE");
        iv_items["3rd"]["nsaid"] = new ItemInfo("소염진통제 복용", "OR_BOOL", "6::A004", "BOOL_BASE");
        iv_items["3rd"]["steroid"] = new ItemInfo("스테로이드제 복용", "OR_BOOL", "6::A005", "BOOL_BASE");
        iv_items["3rd"]["estrogen"] = new ItemInfo("여성호르몬제 복용", "OR_BOOL", "6::A006", "BOOL_BASE");
        iv_items["3rd"]["consistent"] = new ItemInfo("지속적복용약 없음", "OR_BOOL", "6::A007", "BOOL_YESNO");
        iv_items["3rd"]["contrast"] = new ItemInfo("조영제부작용", "MULTI", "6::A027,0 || 6::A028,1", "BOOL_BASE");
        iv_items["3rd"]["allergy"] = new ItemInfo("알레르기", "MULTI", "6::A029,0 || 6::A030,1 || 6::A031,2", "MULTI_KNOW");
        iv_items["3rd"]["helico"] = new ItemInfo("헬리코박터 제균", "MULTI", "9::A016, 2 || 9::A017, 0 || 9::A018, 1", "MULTI_KNOW");
        iv_items["3rd"]["hp_cause"] = new ItemInfo("제균이유", "MULTI", "9::A019,1 || 9::A020,3 || 9::A021,5 || 9::A022,7 || 9::A024,2 || 9::A025,4 || 9::A026,4", "MULTI_hp_cause");
        iv_items["3rd"]["hp_when"] = new ItemInfo("제균시기", "CALC", "FUNC:hp_when_3rd", "");
        iv_items["3rd"]["smoking"] = new ItemInfo("흡연", "MULTI", "7::A147, 0 || 7::A148, 1 || 7::A149, 2", "MULTI_smoking");
        iv_items["3rd"]["smk_py"] = new ItemInfo("흡연량(pack-year)", "CALC", "FUNC:smk_py_3rd", "");
        iv_items["3rd"]["dairy"] = new ItemInfo("유제품 섭취", "OR_BOOL", "16::A010", "BOOL_YESNO");
        iv_items["3rd"]["salty"] = new ItemInfo("음식간을 추가로 한다", "OR_BOOL", "16::A026", "BOOL_YESNO");
        iv_items["3rd"]["alcohol_yes"] = new ItemInfo("현재음주여부", "MULTI", "8::A006,0 || 8::A007,1 || 8::A008,2", "MULTI_alcohol_yes");
        iv_items["3rd"]["alcohol"] = new ItemInfo("주당 음주량", "CALC", "FUNC:alchohol_3rd", "");
        iv_items["3rd"]["education"] = new ItemInfo("학력", "MULTI", "10::A044, 1 || 10::A045, 3 || 10::A046, 5 || 10::A047, 2 || 10::A048, 4", "MULTI_education");
        iv_items["3rd"]["income"] = new ItemInfo("수입", "MULTI", "10::A049, 1 || 10::A050, 3 || 10::A051, 5 || 10::A052, 7 || 10::A053, 2 || 10::A054, 4 || 10::A055, 6 || 10::A056, 8", "MULTI_income");
        iv_items["3rd"]["Menarche"] = new ItemInfo("초경", "TEXT", "13::A003", "");
        iv_items["3rd"]["menarche_school"] = new ItemInfo("초경학년", "CALC", "FUNC:menarche_school_3rd", "");
        iv_items["3rd"]["parity"] = new ItemInfo("분만력", "OR_BOOL", "13::A007", "BOOL_BASE");
        iv_items["3rd"]["menopause"] = new ItemInfo("폐경여부", "MULTI", "13::A019,0 || 13::A020,2 || 13::A021,1", "MULTI_menopause");
        iv_items["3rd"]["memo_surg"] = new ItemInfo("외과적 폐경", "CALC", "FUNC:memo_surg_3rd", "");
        iv_items["3rd"]["HRT"] = new ItemInfo("호르몬치료", "MULTI", "13::A026,1 || 13::A027,2 || 13::A028,0", "MULTI_HRT");
        iv_items["3rd"]["HRT_dur"] = new ItemInfo("호르몬치료기간", "CALC", "FUNC:HRT_dur_3rd", "");
        iv_items["3rd"]["HRT_ex_dur"] = new ItemInfo("중단후 기간", "TEXT", "13::A032", "");
        iv_items["3rd"]["HRT_drug"] = new ItemInfo("호르몬 종류", "TEXT", "13::A030", "");
        iv_items["3rd"]["GI_1"] = new ItemInfo("음식을삼킬때 목이나 가슴부위에 음식이 걸린듯하다", "OR_BOOL", "11::A013", "BOOL_BASE");
        iv_items["3rd"]["GI_2"] = new ItemInfo("구역질이나 구토가 자주난다", "OR_BOOL", "11::A014", "BOOL_BASE");
        iv_items["3rd"]["GI_3"] = new ItemInfo("소화가 안되고, 헛배가 부른다", "OR_BOOL", "11::A015", "BOOL_BASE");
        iv_items["3rd"]["GI_5"] = new ItemInfo("공복시에나 식후에 속이 쓰리다", "OR_BOOL", "11::A016", "BOOL_BASE");
        iv_items["3rd"]["GI_6"] = new ItemInfo("신물이 자주 넘어오고 가슴쪽에 화끈한증상이 있다", "OR_BOOL", "11::A017", "BOOL_BASE");
        iv_items["3rd"]["GI_8"] = new ItemInfo("대변색이 자장처럼 검게 나온적이 있다", "OR_BOOL", "11::A018", "BOOL_BASE");
        iv_items["3rd"]["GI_9"] = new ItemInfo("대변에 붉은 피가 섞여 나온적이 있다", "OR_BOOL", "11::A019", "BOOL_BASE");
        iv_items["3rd"]["GI_10"] = new ItemInfo("최근에 배변습관이 현저하게 바뀌었다", "OR_BOOL", "11::A020", "BOOL_BASE");
        iv_items["3rd"]["wt_loss"] = new ItemInfo("체중감소 여부", "OR_BOOL", "11::A001", "BOOL_BASE");
        iv_items["3rd"]["wt_loss_kg"] = new ItemInfo("체중감소 정도", "CALC", "FUNC:wt_loss_kg_3rd", "");
        iv_items["3rd"]["wt_loss_du"] = new ItemInfo("체중감소 기간", "CALC", "FUNC:wt_loss_du_3rd", "");
        iv_items["3rd"]["FH_HTN"] = new ItemInfo("고혈압 가족력", "OR_BOOL", "7::A019 || 7::A020 || 7::A021 || 7::A022", "BOOL_FH_1st");
        iv_items["3rd"]["FH_CAD"] = new ItemInfo("심근경색/협심증 가족력", "OR_BOOL", "7::A027 || 7::A028 || 7::A029 || 7::A030", "BOOL_FH_1st");
        iv_items["3rd"]["FH_DM"] = new ItemInfo("당뇨 가족력", "OR_BOOL", "7::A035 || 7::A036 || 7::A037 || 7::A038", "BOOL_FH_1st");
        iv_items["3rd"]["FM_stroke"] = new ItemInfo("뇌졸중 가족력", "OR_BOOL", "7::A043 || 7::A044 || 7::A045 || 7::A046", "BOOL_FH_1st");
        iv_items["3rd"]["FM_stom_ca1"] = new ItemInfo("위암 가족력1", "OR_BOOL", "7::A091 || 7::A092 || 7::A093 || 7::A094", "BOOL_FH_1st");
        iv_items["3rd"]["FM_stom_ca2"] = new ItemInfo("위암 가족력2", "OR_BOOL", "7::A095 || 7::A096 || 7::A097 || 7::A098", "BOOL_FH_2nd");
        iv_items["3rd"]["FM_col_ca1"] = new ItemInfo("대장/직장암 가족력1", "OR_BOOL", "7::A099 || 7::A100 || 7::A101 || 7::A102", "BOOL_FH_1st");
        iv_items["3rd"]["FM_col_ca2"] = new ItemInfo("대장/직장암 가족력2", "OR_BOOL", "7::A103 || 7::A104 || 7::A105 || 7::A106", "BOOL_FH_2nd");
        iv_items["3rd"]["IPSS_total"] = new ItemInfo("IPSS 총점", "CALC", "FUNC:ipss_total_3rd", "");
        iv_items["3rd"]["IPSS_inconv"] = new ItemInfo("IPSS 불편감", "MULTI", "13::A043, 0 || 13::A044, 1 || 13::A045, 2 || 13::A046, 3 || 13::A047, 4 || 13::A048, 5 || 13::A049, 6", "MULTI_VALUE");
        iv_items["3rd"]["BDI_total"] = new ItemInfo("BDI 총점", "CALC", "FUNC:bdi_total_3rd", "");

        //--------------------------------------------------------------------------------------------
        // 4차 문진표 항목 ---------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------

        iv_items["4th"] = new Dictionary<string, ItemInfo>();
        iv_items["4th"]["HTN"] = new ItemInfo("고혈압", "OR_BOOL", "2::A003", "BOOL_BASE");
        iv_items["4th"]["HTN_dx"] = new ItemInfo("고혈압 진단력", "OR_BOOL", "2::A002", "BOOL_BASE");
        iv_items["4th"]["CAD"] = new ItemInfo("협심증/심근경색", "OR_BOOL", "2::A009 || 2::A010", "BOOL_BASE");
        iv_items["4th"]["CAD_dx"] = new ItemInfo("협심증/심근경색 진단력", "OR_BOOL", "2::A008", "BOOL_BASE");
        iv_items["4th"]["revas"] = new ItemInfo("재관류술", "OR_BOOL", "2::A010 || 2::A011", "BOOL_BASE");
        iv_items["4th"]["dyslipid"] = new ItemInfo("고지혈증", "OR_BOOL", "2::A007", "BOOL_BASE");
        iv_items["4th"]["dyslipid_dx"] = new ItemInfo("고지혈증 진단력", "OR_BOOL", "2::A006", "BOOL_BASE");
        iv_items["4th"]["DM"] = new ItemInfo("당뇨병", "OR_BOOL", "2::A005", "BOOL_BASE");
        iv_items["4th"]["DM_dx"] = new ItemInfo("당뇨병 진단력", "OR_BOOL", "2::A004", "BOOL_BASE");
        iv_items["4th"]["asthma"] = new ItemInfo("기관지천식", "OR_BOOL", "2::A029", "BOOL_BASE");
        iv_items["4th"]["asthma_dx"] = new ItemInfo("기관지천식 진단력", "OR_BOOL", "2::A028", "BOOL_BASE");
        iv_items["4th"]["all_rhin"] = new ItemInfo("알러지비염", "OR_BOOL", "2::A031", "BOOL_BASE");
        iv_items["4th"]["all_rhin_dx"] = new ItemInfo("알러지비염 진단력", "OR_BOOL", "2::A030", "BOOL_BASE");
        iv_items["4th"]["CRD"] = new ItemInfo("만성신장질환", "OR_BOOL", "2::A015 || 2::A016 || 2::A017", "BOOL_BASE");
        iv_items["4th"]["CRD_dx"] = new ItemInfo("만성신장질환 진단력", "OR_BOOL", "2::A014", "BOOL_BASE");
        iv_items["4th"]["stroke"] = new ItemInfo("뇌졸중/중풍", "OR_BOOL", "2::A013", "BOOL_BASE");
        iv_items["4th"]["stroke_dx"] = new ItemInfo("뇌졸중/중풍 진단력", "OR_BOOL", "2::A012", "BOOL_BASE");
        iv_items["4th"]["cirrhosis"] = new ItemInfo("간경변", "OR_BOOL", "2::A019", "BOOL_BASE");
        iv_items["4th"]["cirrhosis_dx"] = new ItemInfo("간경변 진단력", "OR_BOOL", "2::A018", "BOOL_BASE");
        iv_items["4th"]["HBV"] = new ItemInfo("만성 B형 간염", "OR_BOOL", "2::A021", "BOOL_BASE");
        iv_items["4th"]["HBV_dx"] = new ItemInfo("만성 B형 간염 진단력", "OR_BOOL", "2::A020", "BOOL_BASE");
        iv_items["4th"]["HCV"] = new ItemInfo("만성 C형 간염", "OR_BOOL", "2::A023", "BOOL_BASE");
        iv_items["4th"]["HCV_dx"] = new ItemInfo("만성 C형 간염 진단력", "OR_BOOL", "2::A022", "BOOL_BASE");
        iv_items["4th"]["TB"] = new ItemInfo("폐결핵", "OR_BOOL", "2::A025", "BOOL_BASE");
        iv_items["4th"]["TB_dx"] = new ItemInfo("폐결핵 진단력", "OR_BOOL", "2::A024", "BOOL_BASE");
        iv_items["4th"]["pastTB"] = new ItemInfo("폐결핵치료력", "OR_BOOL", "2::A026", "BOOL_BASE");
        iv_items["4th"]["Tbscar"] = new ItemInfo("폐결핵흔적", "OR_BOOL", "2::A027", "BOOL_BASE");
        iv_items["4th"]["fracture"] = new ItemInfo("골절", "OR_BOOL", "2::A032", "BOOL_BASE");
        iv_items["4th"]["other_ds"] = new ItemInfo("기타질환", "CALC", "FUNC:other_ds_4th", "");
        iv_items["4th"]["other_ds_dx"] = new ItemInfo("기타질환명", "TEXT", "2::A035", "");
        iv_items["4th"]["lung_ca"] = new ItemInfo("폐암", "OR_BOOL", "2::A036", "BOOL_BASE");
        iv_items["4th"]["stoma_ca"] = new ItemInfo("위암", "OR_BOOL", "2::A039", "BOOL_BASE");
        iv_items["4th"]["col_ca"] = new ItemInfo("대장암", "OR_BOOL", "2::A041", "BOOL_BASE");
        iv_items["4th"]["hcc"] = new ItemInfo("간암", "OR_BOOL", "2::A043", "BOOL_BASE");
        iv_items["4th"]["breast_ca"] = new ItemInfo("유방암", "OR_BOOL", "2::A037", "BOOL_BASE");
        iv_items["4th"]["cervix_ca"] = new ItemInfo("자궁경부암", "OR_BOOL", "2::A040", "BOOL_BASE");
        iv_items["4th"]["thyroid_ca"] = new ItemInfo("갑상선암", "OR_BOOL", "2::A042", "BOOL_BASE");
        iv_items["4th"]["prostate_ca"] = new ItemInfo("전립선암", "OR_BOOL", "2::A044", "BOOL_BASE");
        iv_items["4th"]["other_ca"] = new ItemInfo("기타암", "TEXT", "2::A045", "");
        iv_items["4th"]["surg_abd"] = new ItemInfo("개복수술", "OR_BOOL", "2::A046 || 2::A047 || 2::A048 || 2::A049 || 2::A050 || 2::A051 || 2::A052 || 2::A053 || 2::A054", "BOOL_BASE");
        iv_items["4th"]["surg_abd_dx"] = new ItemInfo("개복수술 진단명", "TEXT", "2::A055", "");
        iv_items["4th"]["surg_other"] = new ItemInfo("기타수술", "OR_BOOL", "2::A056", "BOOL_BASE");
        iv_items["4th"]["surg_other_ds"] = new ItemInfo("기타 수술 진단명", "TEXT", "2::A057", "");
        iv_items["4th"]["aspirin"] = new ItemInfo("아스피린 복용", "AND_BOOL", "3::A007 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["anti_coa"] = new ItemInfo("항응고제 복용", "AND_BOOL", "3::A008 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["cal_suppl"] = new ItemInfo("칼슘제 복용", "AND_BOOL", "3::A014 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["nsaid"] = new ItemInfo("소염진통제 복용", "AND_BOOL", "3::A009 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["steroid"] = new ItemInfo("스테로이드제 복용", "AND_BOOL", "3::A010 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["estrogen"] = new ItemInfo("여성호르몬제 복용", "OR_BOOL", "8::A002", "BOOL_BASE");
        iv_items["4th"]["thyroid"] = new ItemInfo("갑상선호르몬", "AND_BOOL", "3::A011 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["anti_thyroid"] = new ItemInfo("갑상선기능항진증약", "AND_BOOL", "3::A012 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["osteoporosis"] = new ItemInfo("골다공증약", "AND_BOOL", "3::A013 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["sedative"] = new ItemInfo("진정제,수면제", "AND_BOOL", "3::A015 || !3::A006", "BOOL_BASE");
        iv_items["4th"]["contrast"] = new ItemInfo("조영제부작용", "OR_BOOL", "1::A016", "BOOL_BASE");
        iv_items["4th"]["allergy"] = new ItemInfo("알레르기", "MULTI", "1::A018", "MULTI_KNOW");
        iv_items["4th"]["al_cause"] = new ItemInfo("알레르기의 원인", "MULTI", "1::A019, 1 || 1::A020, 2", "MULTI_al_cause");
        iv_items["4th"]["helico"] = new ItemInfo("헬리코박터 제균", "MULTI", "3::A001, 0 || 3::A002, 2", "MULTI_KNOW");
        iv_items["4th"]["hp_eradi"] = new ItemInfo("헬리코박터 제균 여부", "MULTI", "3::A003, 0 || 3::A004, 1 || 3::A005, 2", "MULTI_eradi");
        iv_items["4th"]["smoking"] = new ItemInfo("흡연", "MULTI", "3::A028, 0 || 3::A029, 1 || 3::A030, 2", "MULTI_smoking");
        iv_items["4th"]["smk_py"] = new ItemInfo("흡연량(pack-year)", "CALC", "FUNC:smk_py_4th", "");
        iv_items["4th"]["meal_freq"] = new ItemInfo("매끼 식사 여부", "MULTI", "15::A001, 0 || 15::A002, 1 || 15::A003, 2", "MULTI_meal_freq");
        iv_items["4th"]["meal_amount"] = new ItemInfo("식사량", "MULTI", "15::A004, 0 || 15::A005, 1 || 15::A006, 2", "MULTI_meal_amount");
        iv_items["4th"]["meal_out"] = new ItemInfo("외식은 얼마나 자주", "MULTI", "15::A007, 0 || 15::A008, 1 || 15::A009, 2 || 15::A010, 3", "MULTI_meal_out");
        iv_items["4th"]["meal_snack"] = new ItemInfo("간식", "MULTI", "15::A011, 0 || 15::A012, 1 || 15::A013, 2", "MULTI_meal_snack");
        iv_items["4th"]["meal_rice"] = new ItemInfo("한 끼 밥량", "MULTI", "15::A014, 0 || 15::A015, 1 || 15::A016, 2", "MULTI_meal_rice");
        iv_items["4th"]["meal_carbo"] = new ItemInfo("곡류 섭취", "MULTI", "15::A017, 0 || 15::A018, 1 || 15::A019, 2", "MULTI_meal_carbo");
        iv_items["4th"]["meal_prot"] = new ItemInfo("단백질 섭취", "MULTI", "15::A020, 0 || 15::A021, 1 || 15::A022, 2 || 15::A023, 3", "MULTI_meal_prot");
        iv_items["4th"]["meal_veget"] = new ItemInfo("채소 반찬", "MULTI", "15::A024, 0 || 15::A025, 1 || 15::A026, 2 || 15::A027, 3", "MULTI_meal_veget");
        iv_items["4th"]["meal_dairy"] = new ItemInfo("유제품 섭취", "MULTI", "15::A028, 0 || 15::A029, 1 || 15::A030, 2 || 15::A031, 3", "MULTI_meal_dairy");
        iv_items["4th"]["meal_fruit"] = new ItemInfo("과일 섭취", "MULTI", "15::A032, 0 || 15::A033, 1 || 15::A034, 2", "MULTI_meal_fruit");
        iv_items["4th"]["meal_water"] = new ItemInfo("물 섭취", "MULTI", "15::A035, 0 || 15::A036, 1 || 15::A037, 2 || 15::A038, 3", "MULTI_meal_water");
        iv_items["4th"]["meal_fried"] = new ItemInfo("튀김 부침개 섭취", "MULTI", "16::A001, 0 || 16::A002, 1 || 16::A003, 2 || 16::A004, 3", "MULTI_meal_fried");
        iv_items["4th"]["meal_fatty"] = new ItemInfo("지방 육류", "MULTI", "16::A005, 0 || 16::A006, 1 || 16::A007, 2 || 16::A008, 3", "MULTI_meal_fatty");
        iv_items["4th"]["meal_instant"] = new ItemInfo("인스턴트 가공 식품", "MULTI", "16::A009, 0 || 16::A010, 1 || 16::A011, 2 || 16::A012, 3", "MULTI_meal_instant");
        iv_items["4th"]["meal_salty1"] = new ItemInfo("음식간을 추가로 한다", "MULTI", "16::A013, 0 || 16::A014, 1 || 16::A015, 2", "MULTI_meal_salty1");
        iv_items["4th"]["meal_salty2"] = new ItemInfo("젓갈 장아찌", "MULTI", "16::A016, 0 || 16::A017, 1 || 16::A018, 2", "MULTI_meal_salty2");
        iv_items["4th"]["meal_sugar"] = new ItemInfo("단 음식", "MULTI", "16::A019, 0 || 16::A020, 1 || 16::A021, 2", "MULTI_meal_sugar");
        iv_items["4th"]["meal_juice"] = new ItemInfo("주스, 음료, 드링크류", "MULTI", "16::A022, 0 || 16::A023, 1 || 16::A024, 2 || 16::A025, 3", "MULTI_meal_juice");
        iv_items["4th"]["meal_coffee"] = new ItemInfo("커피", "MULTI", "16::A026, 0 || 16::A027, 1 || 16::A028, 2 || 16::A029, 3", "MULTI_meal_coffee");
        iv_items["4th"]["alcohol"] = new ItemInfo("주당 음주량", "CALC", "FUNC:alchohol_4th", "");
        iv_items["4th"]["alc_frequency"] = new ItemInfo("음주 빈도", "MULTI", "4::A001, 0 || 4::A002, 1 || 4::A003, 2 || 4::A004, 3 || 4::A005, 4 || 4::A006, 5 || 4::A007, 6 || 4::A008, 7", "MULTI_alc_frequency");
        iv_items["4th"]["alc_amount"] = new ItemInfo("한번음주량", "MULTI", "4::A009, 0 || 4::A010, 2 || 4::A011, 4 || 4::A012, 1 || 4::A013, 3", "MULTI_alc_amount");
        iv_items["4th"]["exer_heavy"] = new ItemInfo("격렬한 활동 일수", "MULTI", "4::A014, 0 || 4::A015, 2 || 4::A016, 4 || 4::A017, 6 || 4::A018, 1 || 4::A019, 3 || 4::A020, 5 || 4::A021, 7", "MULTI_exer_heavy");
        iv_items["4th"]["exer_heavy_dur"] = new ItemInfo("격렬한 활동 시간", "TEXT", "4::A022", "");
        iv_items["4th"]["exer_mod"] = new ItemInfo("중간정도 활동 일수", "MULTI", "4::A023, 0 || 4::A024, 2 || 4::A025, 4 || 4::A026, 6 || 4::A027, 1 || 4::A028, 3 || 4::A029, 5 || 4::A030, 7", "MULTI_exer_heavy");
        iv_items["4th"]["exer_mod_dur"] = new ItemInfo("중간정도 활동 시간", "TEXT", "4::A031", "");
        iv_items["4th"]["exer_light"] = new ItemInfo("10분이상 걸은 날수", "MULTI", "4::A032, 0 || 4::A033, 2 || 4::A034, 4 || 4::A035, 6 || 4::A036, 1 || 4::A037, 3 || 4::A038, 5 || 4::A039, 7", "MULTI_exer_heavy");
        iv_items["4th"]["exer_light_dur"] = new ItemInfo("걸은 시간", "TEXT", "4::A040", "");
        iv_items["4th"]["exercise"] = new ItemInfo("주당 운동시간", "CALC", "FUNC:exercise_4th", "");
        iv_items["4th"]["exer_MET"] = new ItemInfo("주당 활동량 MET", "CALC", "FUNC:exer_MET_4th", "");
        iv_items["4th"]["education"] = new ItemInfo("학력", "MULTI", "5::A040, 0 || 5::A041, 1 || 5::A042, 2 || 5::A043, 3 || 5::A044, 4", "MULTI_education");
        iv_items["4th"]["income"] = new ItemInfo("수입", "MULTI", "5::A045, 0 || 5::A046, 1 || 5::A047, 2 || 5::A048, 3 || 5::A049, 4 || 5::A050, 5 || 5::A051, 6", "MULTI_income");
        iv_items["4th"]["Menarche"] = new ItemInfo("초경", "TEXT", "9::A001", "");
        iv_items["4th"]["parity"] = new ItemInfo("분만력", "OR_BOOL", "9::A002", "BOOL_BASE");
        iv_items["4th"]["menopause"] = new ItemInfo("폐경여부", "MULTI", "9::A006, 0 || 9::A007, 2 || 9::A008, 1", "MULTI_menopause");
        iv_items["4th"]["memo_surg"] = new ItemInfo("외과적 폐경", "OR_BOOL", "9::A011", "BOOL_YES");
        iv_items["4th"]["HRT"] = new ItemInfo("호르몬치료", "MULTI", "9::A014, 0 || 9::A015, 1 || 9::A016, 2", "MULTI_HRT");
        iv_items["4th"]["HRT_dur"] = new ItemInfo("호르몬치료기간", "CALC", "FUNC:HRT_dur_4th", "");
        iv_items["4th"]["HRT_ex_dur"] = new ItemInfo("중단후 기간", "TEXT", "9::A019", "");
        iv_items["4th"]["FH_HTN"] = new ItemInfo("고혈압 가족력", "OR_BOOL", "3::A019", "BOOL_FH_1st");
        iv_items["4th"]["FH_CAD"] = new ItemInfo("심근경색/협심증 가족력", "OR_BOOL", "3::A021", "BOOL_FH_1st");
        iv_items["4th"]["FH_DM"] = new ItemInfo("당뇨 가족력", "OR_BOOL", "3::A020", "BOOL_FH_1st");
        iv_items["4th"]["FM_stroke"] = new ItemInfo("뇌졸중 가족력", "OR_BOOL", "3::A022", "BOOL_FH_1st");
        iv_items["4th"]["FM_cirrhosis"] = new ItemInfo("만성간염, 간경변 가족력", "OR_BOOL", "3::A023", "BOOL_FH_1st");
        iv_items["4th"]["FM_stom_ca1"] = new ItemInfo("위암 가족력1", "OR_BOOL", "3::A025", "BOOL_FH_1st");
        iv_items["4th"]["FM_col_ca1"] = new ItemInfo("대장/직장암 가족력1", "OR_BOOL", "3::A026", "BOOL_FH_1st");
        iv_items["4th"]["FM_liver_ca"] = new ItemInfo("간암 가족력", "OR_BOOL", "3::A024", "BOOL_FH_1st");
        iv_items["4th"]["FM_lung_ca"] = new ItemInfo("폐암 가족력", "OR_BOOL", "3::A027", "BOOL_FH_1st");

        //--------------------------------------------------------------------------------------------
        // Multi-Select Values -----------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------

        iv_enum = new Dictionary<string, Dictionary<string, string>>();

        iv_enum["BOOL_BASE"] = new Dictionary<string,string>{ {"1", "있다"}, {"0", "없다"} };
        iv_enum["BOOL_YESNO"] = new Dictionary<string, string> { { "1", "예" }, { "0", "아니오" } };
        iv_enum["MULTI_KNOW"] = new Dictionary<string, string> { { "2", "있다" }, { "1", "모른다" }, { "0", "없다" } };
        iv_enum["MULTI_hp_cause"] = new Dictionary<string, string> { { "1", "위궤양" }, { "2", "십이지장궤양" }, { "3", "위, 십이지장궤양 모두" }, { "4", "위염 또는 십이지장염" }, { "5", "위림프종" }, { "6", "큰 이상은 없으나 균이 있다고 해서" }, { "7", "기타" } };
        iv_enum["MULTI_smoking"] = new Dictionary<string, string> { { "2", "현재도 피우고 있다" }, { "1", "이전에 피웠으나 끊었다" }, { "0", "피운적이 없다" } };
        iv_enum["MULTI_alcohol_yes"] = new Dictionary<string, string> { { "0", "원래안마심" }, { "1", "현재중단" }, { "2", "현재음주" } };
        iv_enum["MULTI_education"] = new Dictionary<string, string> { { "1", "중졸미만" }, { "2", "고등중퇴 또는 재학" }, { "3", "고졸 또는 대재/중퇴" }, { "4", "대졸" }, { "5", "대학원 이상" } };
        iv_enum["MULTI_income"] = new Dictionary<string, string> { { "1", "300미만" }, { "2", "300-500" }, { "3", "500-800" }, { "4", "800-1000" }, { "5", "1000-1500" }, { "6", "1500-2000" }, { "7", "2000이상" }, { "8", "기타" } };
        iv_enum["MULTI_menopause"] = new Dictionary<string, string> { { "0", "아니오" }, { "1", "폐경" }, { "2", "이행기" } };
        iv_enum["MULTI_HRT"] = new Dictionary<string, string> { { "0", "아니오" }, { "1", "예" }, { "2", "중단" } };
        iv_enum["BOOL_FH_1st"] = new Dictionary<string, string> { { "1", "1st degree 내에 있다" }, { "0", "없다" } };
        iv_enum["BOOL_FH_2nd"] = new Dictionary<string, string> { { "1", "{외)조부모 있다" }, { "0", "없다" } };
        iv_enum["MULTI_VALUE"] = new Dictionary<string, string> { { "0", "0" }, { "1", "1" }, { "2", "2" }, { "3", "3" }, { "4", "4" }, { "5", "5" }, { "6", "6" }, { "7", "7" }, { "8", "8" }, { "9", "9" }, { "10", "10" } };

        // 4차에서 추가된 항목

        iv_enum["MULTI_al_cause"] = new Dictionary<string, string> { { "1", "약물" }, { "2", "알레르겐" } };
        iv_enum["MULTI_eradi"] = new Dictionary<string, string> { { "0", "제균됨" }, { "1", "제균치료안됨" }, { "2", "모른다" } };
        iv_enum["MULTI_meal_freq"] = new Dictionary<string, string> { { "0", "하루 3끼 먹는다" }, { "1", "끼니를 거를 때가 많다" }, { "2", "불규칙하다" } };
        iv_enum["MULTI_meal_amount"] = new Dictionary<string, string> { { "0", "과식한다" }, { "1", "보통이다" }, { "2", "소식하는 편이다" } };
        iv_enum["MULTI_meal_out"] = new Dictionary<string, string> { { "0", "매일" }, { "1", "주 1~2회" }, { "2", "주 3~6회" }, { "3", "월 1~3회 이하" } };
        iv_enum["MULTI_meal_snack"] = new Dictionary<string, string> { { "0", "매일" }, { "1", "가끔" }, { "2", "거의 먹지 않는다" } };
        iv_enum["MULTI_meal_rice"] = new Dictionary<string, string> { { "0", "1공기보다 많다" }, { "1", "2/3~1공기 정도" }, { "2", "1/2공기 이하" } };
        iv_enum["MULTI_meal_carbo"] = new Dictionary<string, string> { { "0", "하루 3끼" }, { "1", "하루 1~2끼" }, { "2", "주 6회 이하" } };
        iv_enum["MULTI_meal_prot"] = new Dictionary<string, string> { { "0", "하루 2끼 이상" }, { "1", "주 3~6회" }, { "2", "하루 1끼" }, { "3", "주 1~2회 이하" } };
        iv_enum["MULTI_meal_veget"] = new Dictionary<string, string> { { "0", "하루 2끼 이상" }, { "1", "주 3~6회" }, { "2", "하루 1끼" }, { "3", "주 1~2회 이하" } };
        iv_enum["MULTI_meal_dairy"] = new Dictionary<string, string> { { "0", "하루 3컵 이상" }, { "1", "주 3~6컵" }, { "2", "하루 1~2컵" }, { "3", "주 1~2컵 이하" } };
        iv_enum["MULTI_meal_fruit"] = new Dictionary<string, string> { { "0", "매일" }, { "1", "주 3~6회" }, { "2", "주 1~2회 이하" } };
        iv_enum["MULTI_meal_water"] = new Dictionary<string, string> { { "0", "하루 6컵 이상" }, { "1", "하루 2~3컵" }, { "2", "하루 4~5컵" }, { "3", "하루 1컵 이하" } };
        iv_enum["MULTI_meal_fried"] = new Dictionary<string, string> { { "0", "하루 2끼 이상" }, { "1", "주 3~6회" }, { "2", "하루 1끼 정도" }, { "3", "주 1~2회 이하" } };
        iv_enum["MULTI_meal_fatty"] = new Dictionary<string, string> { { "0", "하루 2끼 이상" }, { "1", "주 3~6회" }, { "2", "하루 1끼 정도" }, { "3", "주 1~2회 이하" } };
        iv_enum["MULTI_meal_instant"] = new Dictionary<string, string> { { "0", "하루 2끼 이상" }, { "1", "주 3~6회" }, { "2", "하루 1끼 정도" }, { "3", "주 1~2회 이하" } };
        iv_enum["MULTI_meal_salty1"] = new Dictionary<string, string> { { "0", "자주 넣는다" }, { "1", "가끔 넣는다" }, { "2", "전혀 넣지 않는다" } };
        iv_enum["MULTI_meal_salty2"] = new Dictionary<string, string> { { "0", "그렇다" }, { "1", "보통이다" }, { "2", "그렇지 않다" } };
        iv_enum["MULTI_meal_sugar"] = new Dictionary<string, string> { { "0", "자주 먹는다" }, { "1", "가끔 먹는다" }, { "2", "전혀 먹지 않는다" } };
        iv_enum["MULTI_meal_juice"] = new Dictionary<string, string> { { "0", "하루 2잔 이상" }, { "1", "주 3~6잔" }, { "2", "하루 1잔 정도" }, { "3", "주 1~2잔 이하" } };
        iv_enum["MULTI_meal_coffee"] = new Dictionary<string, string> { { "0", "하루 5잔 이상" }, { "1", "하루 1~2잔" }, { "2", "하루 3~4잔 정도" }, { "3", "주 6잔 이하" } };
        iv_enum["MULTI_alc_frequency"] = new Dictionary<string, string> { { "0", "한 달에 1번 이하" }, { "1", "한달에 2-4번" }, { "2", "일주일에 2번" }, { "3", "일주일에 3번" }, { "4", "일주일에 4번" }, { "5", "일주일에 5번" }, { "6", "일주일에 6번" }, { "7", "일주일에 7번" } };
        iv_enum["MULTI_alc_amount"] = new Dictionary<string, string> { { "0", "1-2잔" }, { "1", "3-4잔" }, { "2", "5-6잔" }, { "3", "7-9잔" }, { "4", "10잔 이상" } };
        iv_enum["MULTI_exer_heavy"] = new Dictionary<string, string> { { "0", "0" }, { "1", "1" }, { "2", "2" }, { "3", "3" }, { "4", "4" }, { "5", "5" }, { "6", "6" }, { "7", "7" } };
    }

    public Dictionary<string, string> decode_iv_item(string iv_ver, Dictionary<int, Dictionary<string, object>> data, List<string> item_code_arr, string iv_sex)
    {
        Dictionary<string, string> ret = new Dictionary<string, string>();

        ItemInfo def;

        foreach (string item_code in item_code_arr)
        {
            if (iv_items.ContainsKey(iv_ver) == true && iv_items[iv_ver].ContainsKey(item_code) == true)
            {
                def = iv_items[iv_ver][item_code];
            }
            else
            {
                ret[item_code] = "-";
                continue;
            }

            if (def.type.Equals("OR_BOOL") == true || def.type.Equals("AND_BOOL") == true)
            {
                // rule = page::field || page::field (..)
                // 해당 page의 field에 값이 있으면 true, data가 없거나 field가 null이거나 0이면 false..

                // rule = page::field || !page::field (..)
                // NOT 연산 추가 가능
                string[] rules = def.rule.Split("||".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                bool ret_bool = false;

                // AND_BOOL 일때 초기값이 false이면 결과값은 무조건 false이므로
                // 초기값을 true로 줌 (조건이 하나라도 false이면 결과값은 false)
                if (def.type.Equals("OR_BOOL") == true)
                {
                    ret_bool = false;
                }
                if (def.type.Equals("AND_BOOL") == true)
                {
                    ret_bool = true;
                }

                for (int i = 0; i < rules.Length; i++)
                {
                    string rule_cur = rules[i].Trim();
                    if (rule_cur.Equals("") == true) continue;

                    bool not_bool = false;
                    if (rule_cur.Substring(0, 1).Equals("!") == true)
                    {
                        not_bool = true;
                        rule_cur = rule_cur.Replace("!", "");
                    }
                    string[] tmp = rule_cur.Split("::".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    int page = int.Parse(tmp[0].Trim());
                    string field = tmp[1].Trim();

                    bool b = false;
                    if (data.ContainsKey(page) == true && data[page].ContainsKey(field) == true)
                    {
                        string val = data[page][field].ToString();
                        if (val.Equals("1") == true || val.Equals("") == false)
                        {
                            b = true;
                        }

                        if (val.Equals("0") == true)
                        {
                            b = false;
                        }
                    }

                    if (not_bool == true) b = !b;

                    if (def.type.Equals("OR_BOOL") == true)
                    {
                        ret_bool = ret_bool || b;
                    }
                    if (def.type.Equals("AND_BOOL") == true)
                    {
                        ret_bool = ret_bool && b;
                    }
                }

                if (ret_bool == true)
                {
                    ret[item_code] = iv_enum[def.enums]["1"];
                }
                else
                {
                    ret[item_code] = iv_enum[def.enums]["0"];
                }
            }

            if (def.type.Equals("TEXT") == true)
            {
                // 해당 field의 값을 그대로 출력..
                string[] tmp = def.rule.Split("::".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                int page = int.Parse(tmp[0].Trim());
                string field = tmp[1].Trim();

                if (data.ContainsKey(page) == true && data[page].ContainsKey(field) == true)
                {
                    string val = data[page][field].ToString();
                    if (val == null || val.Equals("") == true)
                    {
                        ret[item_code] = "무응답";
                    }
                    else
                    {
                        ret[item_code] = data[page][field].ToString();
                    }
                }
            }

            if (def.type.Equals("MULTI") == true)
            {
                // rule = page::field1, value1 || page::field2, value2 (..)
                // 해당 page1의 field1에 값이 있으면 value1 출력..
                // 여러 field에 값이 다 있으면 ,를 붙여 출력..
                // 모든 data가 없거나 field가 null이거나 0이면 출력값 없음..
                string[] rules = def.rule.Split("||".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

                List<string> ret_multi = new List<string>();

                for (int i = 0; i < rules.Length; i++)
                {
                    if (rules[i].Equals("") == true) continue;

                    // page::field, value
                    string[] cond_value = rules[i].Trim().Split(',');
                    string value = cond_value[1].Trim();
                    string[] page_field = cond_value[0].Trim().Split("::".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    int page = int.Parse(page_field[0].Trim());
                    string field = page_field[1].Trim();

                    if (data.ContainsKey(page) == true && data[page].ContainsKey(field) == true)
                    {
                        string val = data[page][field].ToString();
                        if (val.Equals("0") == false && val != null)
                        {
                            ret_multi.Add(iv_enum[def.enums][value]);
                        }
                    }
                }

                if (ret_multi.Count > 0)
                {
                    // 응답한 경우가 하나라도 있으면..
                    ret[item_code] = string.Join(", ", ret_multi.ToArray());
                }
                else
                {
                    // 응답한 경우가 전혀 없으면..
                    ret[item_code] = "무응답";
                }
            }

            if (def.type.Equals("CALC") == true)
            {
                CALCFUNC callback;

                string[] sw_func = def.rule.Trim().Split(':');
                string sw = sw_func[0].Trim();
                string func = sw_func[1].Trim();
                if (sw.Equals("FUNC") == true)
                {
                    if (func.Equals("smk_py_2nd") == true) { callback = smk_py_2nd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("memo_surg_2nd") == true) { callback = memo_surg_2nd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("ipss_total_2nd") == true) { callback = ipss_total_2nd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("wt_loss_du_3rd") == true) { callback = wt_loss_du_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("wt_loss_kg_3rd") == true) { callback = wt_loss_kg_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("smk_py_3rd") == true) { callback = smk_py_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("alchohol_3rd") == true) { callback = alchohol_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("memo_surg_3rd") == true) { callback = memo_surg_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("hp_when_3rd") == true) { callback = hp_when_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("HRT_dur_3rd") == true) { callback = HRT_dur_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("menarche_school_3rd") == true) { callback = menarche_school_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("ipss_total_3rd") == true) { callback = ipss_total_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("bdi_total_3rd") == true) { callback = bdi_total_3rd; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("other_ds_4th") == true) { callback = other_ds_4th; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("smk_py_4th") == true) { callback = smk_py_4th; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("alcohol_4th") == true) { callback = alcohol_4th; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("exercise_4th") == true) { callback = exercise_4th; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("exer_MET_4th") == true) { callback = exer_MET_4th; ret[item_code] = callback(data, iv_sex); }
                    if (func.Equals("HRT_dur_4th") == true) { callback = HRT_dur_4th; ret[item_code] = callback(data, iv_sex); }
                }
            }

        } // endforeach

        return ret;
    }

    public delegate string CALCFUNC(Dictionary<int, Dictionary<string,object>> data, string iv_sex);

    public bool is_true(object value)
    {
        string val = (string)value;
        val = val.Trim();

        if (val.Equals("") == false)
        {
            if (val.Equals("0") == true) return false;
            else
            {
                return true;
            }
        }

        return false;
    }

    //--------------------------------------------------------------------------------------------
    // 2차 문진표 custom calc function -----------------------------------------------------------
    //--------------------------------------------------------------------------------------------

    string s(object data)
    {
        if (System.DBNull.Value == data) return "";
        else return data.ToString().Trim();
    }

    string getdata(Dictionary<int, Dictionary<string, object>> data, int page, string code)
    {
        string ret = "";
        if (data.ContainsKey(page) == true)
        {
            if (data[6].ContainsKey(code) == true)
            {
                return s(data[page][code]);
            }
        }
        return ret;
    }

    int getintdata_orzero(Dictionary<int, Dictionary<string, object>> data, int page, string code)
    {
        int r = 0;
        if (data.ContainsKey(page) == true)
        {
            if (data[6].ContainsKey(code) == true)
            {
                string value = s(data[page][code]);
                if (Int32.TryParse(value, out r) == true)
                {
                    return r;
                }
                else
                {
                    Debug.WriteLine("PAGE " + page + "/CODE " + code + " VALUE (" + value + ") INT PARSE ERROR");
                }
            }
        }
        return 0;
    }

    string smk_py_2nd(Dictionary<int, Dictionary<string, object>> data, string iv_sex)
    {
        string non_smoker = getdata(data, 6, "A069");
        string now_smoke_no = getdata(data, 6, "A070");
        string now_smoke_yes = getdata(data, 6, "A077");

        string smoke_pack_no = getdata(data, 6, "A075");
        string smoke_cigar_no = getdata(data, 6, "A076");

        string smoke_pack = getdata(data, 6, "A078");
        string smoke_cigar = getdata(data, 6, "A079");

        double smoke_py = 0;
        if (is_true(getdata(data, 6, "A080")) == true) smoke_py = 2.5;
        if (is_true(getdata(data, 6, "A081")) == true) smoke_py = 7.5;
        if (is_true(getdata(data, 6, "A082")) == true) smoke_py = 15;
        if (is_true(getdata(data, 6, "A083")) == true) smoke_py = 30;

        if (is_true(now_smoke_no) == true && is_true(now_smoke_yes) == true)
        {
            return "복수답변";
        }

        if (smoke_py == 0)
        {
            return "ERR(기간 없음)";
        }
        else
        {
            double val;
            if (is_true(now_smoke_yes) && smoke_py != 0)
            {
                if (double.TryParse(smoke_cigar, out val))
                {
                    return Math.Round(smoke_py * (val / 20)).ToString();
                }
                else return "ERR(문자포함:" + smoke_cigar + ")";
            }

            if (is_true(now_smoke_yes) && smoke_py != 0)
            {
                if (double.TryParse(smoke_pack, out val))
                {
                    return Math.Round(smoke_py * val).ToString();
                }
                else return "ERR(문자포함:" + smoke_pack + ")";
            }

            if (is_true(now_smoke_no) && smoke_py != 0)
            {
                if (double.TryParse(smoke_cigar_no, out val))
                {
                    return Math.Round(smoke_py * (val / 20)).ToString();
                }
                else return "ERR(문자포함:" + smoke_cigar_no + ")";
            }

            if (is_true(now_smoke_no) && smoke_py != 0)
            {
                if (double.TryParse(smoke_pack_no, out val))
                {
                    return Math.Round(smoke_py * val).ToString();
                }
                else return "ERR(문자포함:" + smoke_pack_no + ")";
            }
        }

        return "";
    }


    string memo_surg_2nd(Dictionary<int, Dictionary<string, object>> data, string iv_sex)
    {
        string ret = "";

        string mp_surg_age = getdata(data, 21, "A044");
        double val = 0;

        if (is_true(mp_surg_age) && double.TryParse(mp_surg_age, out val) == true)
        {
            ret = "예/";
            ret += mp_surg_age + "세";
        }
        else ret = "아니오";

        return ret;
    }

    string ipss_total_2nd(Dictionary<int, Dictionary<string, object>> data, string iv_sex)
    {
        double total = 0, tmp = 0;

        for (int i = 1; i <= 7; i++)
        {
            if (double.TryParse(getdata(data, 16, "A00" + i), out tmp) == true)
            {
                total += tmp;
            }
        }

        return total.ToString();
    }

    //--------------------------------------------------------------------------------------------
    // 3차 문진표 custom calc function -----------------------------------------------------------
    //--------------------------------------------------------------------------------------------

    string wt_loss_du_3rd(Dictionary<int, Dictionary<string, object>> data, string iv_sex)
    {
        string ret = "";

        if (iv_sex.Equals("M") == true)
        {
            ret += getdata(data, 11, "A009");
        }
        else
        {
            ret += getdata(data, 11, "A007");
        }
        ret = ret.Trim();

        if (ret.Equals("") == false) ret += " 개월동안";
        else ret = "무응답";

        return ret;
    }

    string wt_loss_kg_3rd(Dictionary<int, Dictionary<string, object>> data, string iv_sex)
    {
        string ret = "";

        if (iv_sex.Trim().Equals("M") == true)
        {
            ret += getdata(data, 11, "A010");
        }
        else
        {
            ret += getdata(data, 11, "A008");
        }
        ret = ret.Trim();

        if (ret.Equals("") == false) ret += " kg";
        else ret = "무응답";

        return ret;
    }

    string smk_py_3rd(Dictionary<int, Dictionary<string, object>> data, string iv_sex)
    {

        string non_smoker = getdata(data, 7, "A147");
        string now_smoke_no = getdata(data, 7, "A148");
        string now_smoke_yes = getdata(data, 7, "A149");

        string smoke_py = getdata(data, 7, "A151");
        string smoke_cigar = getdata(data, 7, "A152");
        string smoke_pack = getdata(data, 7, "A153");

        string smoke_py_no = getdata(data, 8, "A002");
        string smoke_cigar_no = getdata(data, 8, "A003");
        string smoke_pack_no = getdata(data, 8, "A004");

        if (is_true(non_smoker) == true)
        {
            return "비흡연자";
        }

        if (is_true(now_smoke_no) == true && is_true(now_smoke_yes) == true)
        {
            return "복수답변";
        }

        double py = 0, val2 = 0;

        if (is_true(now_smoke_yes) == true)
        {
            if (smoke_py.Equals("") == false)
            {
                if (smoke_pack.Equals("") == false)
                {
                    if (double.TryParse(smoke_py, out py) && double.TryParse(smoke_pack, out val2))
                    {
                        return Math.Round(py * val2, 1).ToString();
                    }
                    else return "ERR(문자포함:" + smoke_pack + ")";
                }
                else if (smoke_cigar.Equals("") == false)
                {
                    if (double.TryParse(smoke_py, out py) && double.TryParse(smoke_cigar, out val2))
                    {
                        return Math.Round((val2 / 20) * py, 1).ToString();
                    }
                    else return "ERR(문자포함:" + smoke_cigar + ")";
                }
                else
                {
                    //return "현재흡연/흡연량무응답";
                    return "무응답(흡연량)";
                }
            }
            else
            {
                //return "현재흡연/흡연기간무응답";
                return "무응답(흡연기간)";
            }
        }

        if (is_true(now_smoke_no))
        {
            if (smoke_py_no.Equals("") == false)
            {
                if (smoke_pack_no.Equals("") == false)
                {
                    if (double.TryParse(smoke_py_no, out py) && double.TryParse(smoke_pack_no, out val2))
                    {
                        return Math.Round(val2 * py, 1).ToString();
                    }
                    else return "ERR(문자포함:" + smoke_pack_no + ")";
                }
                else if (smoke_cigar_no.Equals("") == false)
                {
                    if (double.TryParse(smoke_py_no, out py) && double.TryParse(smoke_cigar_no, out val2))
                    {
                        return Math.Round((val2 / 20) * py, 1).ToString();
                    }
                    else return "ERR(문자포함:" + smoke_cigar_no + ")";
                }
                else
                {
                    //return "흡연중단/흡연량무응답";
                    return "무응답(흡연량)";
                }
            }
            else
            {
                //return "흡연중단/흡연기간무응답";
                return "무응답(흡연기간)";
            }
        }

        return "무응답";
    }

    double alcohol_calc(double dosu, double amt_cc, string mon, string week, string bottle, string cup, double cups_from_bottle)
    {
        if ((mon.Equals("") == false || week.Equals("") == false) && (bottle.Equals("") == false || cup.Equals("") == false))
        {
            double dMon = 0, dWeek = 0;
            if ((mon.Equals("") == false && double.TryParse(mon, out dMon) != false) || (week.Equals("") == false && double.TryParse(week, out dWeek) != false))
            {
                //return "COUNT_NOT_NUMBER(" + mon + week + ")";
                return -1;
            }

            double dBottle = 0, dCup = 0;
            if ((bottle.Equals("") == false && double.TryParse(bottle, out dBottle)) || (cup.Equals("") == false && double.TryParse(cup, out dCup)))
            {
                //return "AMOUNT_NOT_NUMBER(" + bottle + cup + ")";
                return -1;
            }

            return Math.Round(dosu * amt_cc * 0.79 * ((dMon * 0.25) + dWeek) * (dBottle + (dCup / cups_from_bottle)), 3);
            //else return "AMOUNT_NOT_NUMBER(" + dosu + amt_cc + cups_from_bottle + ")";
        }
        //else return "NULL_ERR";
        else return -1;
    }

    string alchohol_3rd(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        List<string> ret = new List<string>();

        string no_drink = getdata(data, 8, "A006");
        string now_drink_no = getdata(data, 8, "A007");
        string now_drink_yes = getdata(data, 8, "A008");

        string drink_py = getdata(data, 8, "A010");

        double total = 0;

        bool error = false;

        // 소주
        double dosu = 0.21;
        double amt_cc = 360;
        string mon = getdata(data, 8, "A012");
        string week = getdata(data, 8, "A013");
        string a_bot = getdata(data, 8, "A014");
        string a_cup = getdata(data, 8, "A015");

        double amt = alcohol_calc(dosu, amt_cc, mon, week, a_bot, a_cup, 7);
        if (amt != -1)
        {
            ret.Add("소주: " + amt.ToString());
            total += amt;
        }
        else error = true;

        // 맥주
        dosu = 0.05;
        amt_cc = 500;
        mon = getdata(data, 8, "A016");
        week = getdata(data, 8, "A017");
        a_bot = getdata(data, 8, "A018");
        a_cup = getdata(data, 8, "A019");

        amt = alcohol_calc(dosu, amt_cc, mon, week, a_bot, a_cup, 2);
        if (amt != -1)
        {
            ret.Add("맥주: " + amt);
            total += amt;
        }
        else error = true;

        // 포도주
        dosu = 0.12;
        amt_cc = 750;
        mon = getdata(data, 8, "A020");
        week = getdata(data, 8, "A021");
        a_bot = getdata(data, 8, "A022");
        a_cup = getdata(data, 8, "A023");

        amt = alcohol_calc(dosu, amt_cc, mon, week, a_bot, a_cup, 8);
        if (amt != -1)
        {
            ret.Add("포도주: " + amt);
            total += amt;
        }
        else error = true;

        // 양주
        dosu = 0.4;
        amt_cc = 500;
        mon = getdata(data, 8, "A024");
        week = getdata(data, 8, "A025");
        a_bot = getdata(data, 8, "A026");
        a_cup = getdata(data, 8, "A027");

        amt = alcohol_calc(dosu, amt_cc, mon, week, a_bot, a_cup, 16);
        if (amt != -1)
        {
            ret.Add("양주: " + amt);
            total += amt;
        }
        else error = true;

        // 기타
        dosu = 0.12;
        amt_cc = 750;
        string etc = getdata(data, 8, "A011");
        mon = getdata(data, 8, "A028");
        week = getdata(data, 8, "A029");
        a_bot = getdata(data, 8, "A030");
        a_cup = getdata(data, 8, "A031");

        amt = alcohol_calc(dosu, amt_cc, mon, week, a_bot, a_cup, 7);
        if (amt != -1)
        {
            ret.Add("(기타) " + etc + ": " + amt);
            total += amt;
        }

        if (ret.Count > 0)
        {
            if (error == true)
            {
                return "ERROR";
            }
            else return "총 " + total + "\n" + string.Join("\n", ret);
        }
        return "";
    }

    string memo_surg_3rd(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        string ret = "";

        string menopause_no = getdata(data, 13, "A021");
        string mp_surg_age = getdata(data, 13, "A024");
        string mp_surg_year = getdata(data, 13, "A025");

        if (is_true(menopause_no))
        {
            List<string> ret_tmp = new List<string>() { "예" };
            if (mp_surg_age.Equals("")==false) ret_tmp.Add(mp_surg_age + "세");
            if (mp_surg_year.Equals("") == false) ret_tmp.Add(mp_surg_year + "년");
            ret = string.Join("/", ret_tmp);
        }
        else
        {
            ret = "아니오";
        }

        return ret;
    }

    string hp_when_3rd(Dictionary<int, Dictionary<string,object>> data, string iv_sex) {
		string ret = "";

        string hp_when = getdata(data, 9, "A027");

        int y;
        if (hp_when.Length == 2 && int.TryParse(hp_when, out y) == true)
        {
            DateTime d = DateTime.Now;
			// 년도가 두자리수일때..
			if ( y + 2000 >=  d.Year ) {
                // 금년도보다 뒷자리가 크면 2000년도 이전..
                ret = "19" + hp_when;
			}
			else {
                // 금년도보다 뒷자리가 작으면 2000년도 이후..
                ret = "20" + hp_when;
			}
		}
		else {
			// 네자리라면 고칠 필요가 없고..
			// 두자리도 네자리도 아니라면 프로그램에서 손 댈 수 없음..
			ret = hp_when;
		}

        if (hp_when.Equals("") == true) ret = "무응답";

		return ret;
	}

    string HRT_dur_3rd(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        List<string> ret = new List<string>();

        string HRT_yes = getdata(data, 13, "A026");
        string HRT_stop = getdata(data, 13, "A027");
        string HRT_no = getdata(data, 13, "A028");

        if (is_true(HRT_yes) == true || is_true(HRT_stop) == true)
        {
            if (getdata(data, 13, "A029") != "") ret.Add("복용중(복용기간): " + getdata(data, 13, "A029"));
            if (getdata(data, 13, "A031") != "") ret.Add("중단(전체복용기간): " + getdata(data, 13, "A031"));
        }

        return string.Join("\n", ret);
    }

    string menarche_school_3rd(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        string ret = "";

        string school = getdata(data, 13, "A004");
        string sch_year = getdata(data, 13, "A005");

        if (school.Equals("") == false || sch_year.Equals("") == false)
        {
            ret = school + "학교 " + sch_year + "학년";
        }
        else ret = "무응답";

        return ret;
    }


    string ipss_total_3rd(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        int total = 0;

        // no. 1(1-6), 0~5 point
        int start_no = 0, point = 0;
        for (int i = 1; i <= 6; i++)
        {
            if (is_true(getdata(data, 13, "A00" + (i + start_no))) == true) point = i - 1;
        }
        total += point;

        // no. 2(7-12), 0~5 point
        start_no = 6;
        for (int i = 1; i <= 6; i++)
        {
            if (is_true(getdata(data, 13, "A00" + (i + start_no))) == true) point = i - 1;
        }
        total += point;

        // no. 3(13-18), 0~5 point
        start_no = 12;
        for (int i = 1; i <= 6; i++)
        {
            if (is_true(getdata(data, 13, "A00" + (i + start_no))) == true) point = i - 1;
        }
        total += point;

        // no. 4(19-24), 0~5 point
        start_no = 18;
        for (int i = 1; i <= 6; i++)
        {
            if (is_true(getdata(data, 13, "A00" + (i + start_no))) == true) point = i - 1;
        }
        total += point;

        // no. 5(25-30), 0~5 point
        start_no = 24;
        for (int i = 1; i <= 6; i++)
        {
            if (is_true(getdata(data, 13, "A00" + (i + start_no))) == true) point = i - 1;
        }
        total += point;

        // no. 6(31-36), 0~5 point
        start_no = 30;
        for (int i = 1; i <= 6; i++)
        {
            if (is_true(getdata(data, 13, "A00" + (i + start_no))) == true) point = i - 1;
        }
        total += point;

        // no. 7(37-42), 0~5 point
        start_no = 36;
        for (int i = 1; i <= 6; i++)
        {
            if (is_true(getdata(data, 13, "A00" + (i + start_no))) == true) point = i - 1;
        }
        total += point;

        return total.ToString();
    }

    string bdi_total_3rd(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        int total = 0;

        // 16 page - 각각 0-3점으로 계산++
        // 실제 database에서 page는 표기된 페이지보다 +2 되어 있음++
        try
        {
            total += (getintdata_orzero(data, 18, "A001"));
            total += (getintdata_orzero(data, 18, "A002"));
            total += (getintdata_orzero(data, 18, "A003"));
            total += (getintdata_orzero(data, 18, "A004"));
            total += (getintdata_orzero(data, 18, "A005"));
            total += (getintdata_orzero(data, 18, "A006"));

            // 17 page
            total += (getintdata_orzero(data, 19, "A001"));
            total += (getintdata_orzero(data, 19, "A002"));
            total += (getintdata_orzero(data, 19, "A003"));
            total += (getintdata_orzero(data, 19, "A004"));
            total += (getintdata_orzero(data, 19, "A005"));
            total += (getintdata_orzero(data, 19, "A006"));
            total += (getintdata_orzero(data, 19, "A007"));
            total += (getintdata_orzero(data, 19, "A008"));

            // 18 page
            total += (getintdata_orzero(data, 20, "A001"));
            total += (getintdata_orzero(data, 20, "A002"));
            total += (getintdata_orzero(data, 20, "A003"));
            total += (getintdata_orzero(data, 20, "A004"));
            total += (getintdata_orzero(data, 20, "A005"));

            // 6, 7번은 (현재 음식조절로 체중을 줄이고 있다->예/아니오) 임++

            total += (getintdata_orzero(data, 20, "A008"));
            total += (getintdata_orzero(data, 20, "A009"));
        }
        catch (FormatException e)
        {
            Debug.WriteLine(e.Message);
            return "숫자입력오류";
        }

        return total.ToString();

    }

    //--------------------------------------------------------------------------------------------
    // 4차 문진표 custom calc function -----------------------------------------------------------
    //--------------------------------------------------------------------------------------------

    string other_ds_4th(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        // 기타질환 있음 체크 혹은 기타질환명이 주어지면 있다/ 아니면 없다 판정
        if (is_true(getdata(data, 2, "A033")) == true || (getdata(data, 2, "A035")).Trim().Equals("") == true) return "있다";
        else return "없다";
    }

    string smk_py_4th(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        // 현재 흡연중이 아님
        if (is_true(getdata(data, 3, "A028")) == true) return "0";

        // 현재는 끊었음
        if (is_true(getdata(data, 3, "A029")) == true)
        {
            double d32, d33;
            if (double.TryParse(getdata(data, 3, "A032"), out d32) == true && double.TryParse(getdata(data, 3, "A033"), out d33) == true)
            {
                double past_smoke = d32 * (d33 / 20);
                return past_smoke.ToString();
            }
            else return "수치입력오류(" + getdata(data, 3, "A032") + "," + getdata(data, 3, "A033") + ")";
        }

        // 현재 흡연 중
        if (is_true(getdata(data, 3, "A030"))==true)
        {
            double d34, d35;
            if (double.TryParse(getdata(data, 3, "A034"), out d34) == true && double.TryParse(getdata(data, 3, "A035"), out d35) == true)
            {
                double cur_smoke = d34 * (d35 / 20);
                return cur_smoke.ToString();
            }
            else return "수치입력오류(" + getdata(data, 3, "A034") + "," + getdata(data, 3, "A035") + ")";
        }

        return "무응답";
    }


    string alcohol_4th(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        string drink_lt_1 = getdata(data, 4, "A003");
        string drink_2_to_4 = getdata(data, 4, "A003");
        string drink_2 = getdata(data, 4, "A003");
        string drink_4 = getdata(data, 4, "A004");
        string drink_6 = getdata(data, 4, "A005");
        string drink_3 = getdata(data, 4, "A006");
        string drink_5 = getdata(data, 4, "A007");
        string drink_7 = getdata(data, 4, "A008");

        double drink_count = 0;
        if (is_true(drink_lt_1)) drink_count = 0 + 25;
        if (is_true(drink_2_to_4)) drink_count = 0 + 5;
        if (is_true(drink_2)) drink_count = 2;
        if (is_true(drink_4)) drink_count = 4;
        if (is_true(drink_6)) drink_count = 6;
        if (is_true(drink_3)) drink_count = 3;
        if (is_true(drink_5)) drink_count = 5;
        if (is_true(drink_7)) drink_count = 7;

        string drink_amt_1_2 = getdata(data, 4, "A009");
        string drink_amt_5_6 = getdata(data, 4, "A010");
        string drink_amt_gt_10 = getdata(data, 4, "A011");
        string drink_amt_3_4 = getdata(data, 4, "A012");
        string drink_amt_7_9 = getdata(data, 4, "A013");

        double drink_amt = 0;
        if (is_true(drink_amt_1_2)) drink_amt = 1 + 5;
        if (is_true(drink_amt_5_6)) drink_amt = 5 + 5;
        if (is_true(drink_amt_gt_10)) drink_amt = 10;
        if (is_true(drink_amt_3_4)) drink_amt = 3 + 5;
        if (is_true(drink_amt_7_9)) drink_amt = 8;

        return (drink_count * drink_amt).ToString();
    }


    string exercise_4th(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        string ex_hard_day_0 = getdata(data, 4, "A014");
        string ex_hard_day_2 = getdata(data, 4, "A015");
        string ex_hard_day_4 = getdata(data, 4, "A016");
        string ex_hard_day_6 = getdata(data, 4, "A017");
        string ex_hard_day_1 = getdata(data, 4, "A018");
        string ex_hard_day_3 = getdata(data, 4, "A019");
        string ex_hard_day_5 = getdata(data, 4, "A020");
        string ex_hard_day_7 = getdata(data, 4, "A021");

        double hard_day = 0;
        if (is_true(ex_hard_day_0)) hard_day = 0;
        if (is_true(ex_hard_day_2)) hard_day = 2;
        if (is_true(ex_hard_day_4)) hard_day = 4;
        if (is_true(ex_hard_day_6)) hard_day = 6;
        if (is_true(ex_hard_day_1)) hard_day = 1;
        if (is_true(ex_hard_day_3)) hard_day = 3;
        if (is_true(ex_hard_day_5)) hard_day = 5;
        if (is_true(ex_hard_day_7)) hard_day = 7;

        bool parse_ok = true;
        double hard_hour = 0;
        parse_ok &= double.TryParse(getdata(data, 4, "A022"), out hard_hour);

        string ex_mid_day_0 = getdata(data, 4, "A023");
        string ex_mid_day_2 = getdata(data, 4, "A024");
        string ex_mid_day_4 = getdata(data, 4, "A025");
        string ex_mid_day_6 = getdata(data, 4, "A026");
        string ex_mid_day_1 = getdata(data, 4, "A027");
        string ex_mid_day_3 = getdata(data, 4, "A028");
        string ex_mid_day_5 = getdata(data, 4, "A029");
        string ex_mid_day_7 = getdata(data, 4, "A030");

        double mid_day = 0;
        if (is_true(ex_mid_day_0)) mid_day = 0;
        if (is_true(ex_mid_day_2)) mid_day = 2;
        if (is_true(ex_mid_day_4)) mid_day = 4;
        if (is_true(ex_mid_day_6)) mid_day = 6;
        if (is_true(ex_mid_day_1)) mid_day = 1;
        if (is_true(ex_mid_day_3)) mid_day = 3;
        if (is_true(ex_mid_day_5)) mid_day = 5;
        if (is_true(ex_mid_day_7)) mid_day = 7;

        double mid_hour = 0;
        parse_ok &= double.TryParse(getdata(data, 4, "A031"), out mid_hour);

        if (parse_ok)
        {
            return ((hard_day * hard_hour) + (mid_day * mid_hour)).ToString();
        }
        else
        {
            return "수치입력오류(" + getdata(data, 4, "A022") + "," + getdata(data, 4, "A031") + ")";
        }
    }


    string exer_MET_4th(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        string ex_hard_day_0 = getdata(data, 4, "A014");
        string ex_hard_day_2 = getdata(data, 4, "A015");
        string ex_hard_day_4 = getdata(data, 4, "A016");
        string ex_hard_day_6 = getdata(data, 4, "A017");
        string ex_hard_day_1 = getdata(data, 4, "A018");
        string ex_hard_day_3 = getdata(data, 4, "A019");
        string ex_hard_day_5 = getdata(data, 4, "A020");
        string ex_hard_day_7 = getdata(data, 4, "A021");

        double hard_day = 0;
        if (is_true(ex_hard_day_0)) hard_day = 0;
        if (is_true(ex_hard_day_2)) hard_day = 2;
        if (is_true(ex_hard_day_4)) hard_day = 4;
        if (is_true(ex_hard_day_6)) hard_day = 6;
        if (is_true(ex_hard_day_1)) hard_day = 1;
        if (is_true(ex_hard_day_3)) hard_day = 3;
        if (is_true(ex_hard_day_5)) hard_day = 5;
        if (is_true(ex_hard_day_7)) hard_day = 7;

        bool parse_ok = true;
        double hard_hour = 0;
        parse_ok &= double.TryParse(getdata(data, 4, "A022"), out hard_hour);

        string ex_mid_day_0 = getdata(data, 4, "A023");
        string ex_mid_day_2 = getdata(data, 4, "A024");
        string ex_mid_day_4 = getdata(data, 4, "A025");
        string ex_mid_day_6 = getdata(data, 4, "A026");
        string ex_mid_day_1 = getdata(data, 4, "A027");
        string ex_mid_day_3 = getdata(data, 4, "A028");
        string ex_mid_day_5 = getdata(data, 4, "A029");
        string ex_mid_day_7 = getdata(data, 4, "A030");

        double mid_day = 0;
        if (is_true(ex_mid_day_0)) mid_day = 0;
        if (is_true(ex_mid_day_2)) mid_day = 2;
        if (is_true(ex_mid_day_4)) mid_day = 4;
        if (is_true(ex_mid_day_6)) mid_day = 6;
        if (is_true(ex_mid_day_1)) mid_day = 1;
        if (is_true(ex_mid_day_3)) mid_day = 3;
        if (is_true(ex_mid_day_5)) mid_day = 5;
        if (is_true(ex_mid_day_7)) mid_day = 7;

        double mid_hour = 0;
        parse_ok &= double.TryParse(getdata(data, 4, "A031"), out mid_hour);

        string ex_walk_day_0 = getdata(data, 4, "A032");
        string ex_walk_day_2 = getdata(data, 4, "A033");
        string ex_walk_day_4 = getdata(data, 4, "A034");
        string ex_walk_day_6 = getdata(data, 4, "A035");
        string ex_walk_day_1 = getdata(data, 4, "A036");
        string ex_walk_day_3 = getdata(data, 4, "A037");
        string ex_walk_day_5 = getdata(data, 4, "A038");
        string ex_walk_day_7 = getdata(data, 4, "A039");

        double walk_day = 0;
        if (is_true(ex_walk_day_0)) walk_day = 0;
        if (is_true(ex_walk_day_2)) walk_day = 2;
        if (is_true(ex_walk_day_4)) walk_day = 4;
        if (is_true(ex_walk_day_6)) walk_day = 6;
        if (is_true(ex_walk_day_1)) walk_day = 1;
        if (is_true(ex_walk_day_3)) walk_day = 3;
        if (is_true(ex_walk_day_5)) walk_day = 5;
        if (is_true(ex_walk_day_7)) walk_day = 7;

        double walk_hour = 0;
        parse_ok &= double.TryParse(getdata(data, 4, "A040"), out walk_hour);

        if (parse_ok)
        {
            return (hard_day * hard_hour * 8 + 0) + (mid_day * mid_hour * 4 + 0) + (walk_day * walk_hour * 3 + 3).ToString();
        }
        else
        {
            return "수치입력오류(" + getdata(data, 4, "A022") + "," + getdata(data, 4, "A031") + "," + getdata(data, 4, "A040") + ")";
        }
    }
    
    string HRT_dur_4th(Dictionary<int, Dictionary<string,object>> data, string iv_sex)
    {
        List<string> ret = new List<string>();

        string HRT_yes = getdata(data, 9, "A015");
        string HRT_stop = getdata(data, 9, "A016");
        string HRT_no = getdata(data, 9, "A014");

        if (is_true(HRT_yes) || is_true(HRT_stop))
        {
            if (getdata(data, 9, "A017").Equals("") == false) ret.Add("복용중(복용기간): " + getdata(data, 9, "A017"));
            if (getdata(data, 9, "A018").Equals("") == false) ret.Add("중단(전체복용기간): " + getdata(data, 9, "A017"));
        }

        return string.Join("\n", ret);
    }

    
}