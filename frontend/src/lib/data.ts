import { format, addDays, differenceInDays } from 'date-fns';
import type { Incident } from "@/lib/types";

export const organizations = ["本社A", "本社B", "東日本", "西日本"] as const;
export const creators = ["山田 太郎", "鈴木 次郎", "田中 三郎", "佐藤 花子", "高橋 一郎", "伊藤 美咲", "渡辺 健太", "山本 さくら", "中村 雄介", "小林 あゆみ"];
export const occurrenceLocations = ["倉庫（入荷作業）", "倉庫（格納作業）", "倉庫（出荷作業）", "配送（集荷/配達）", "配送（施設内）", "お客様先"] as const;
export const shippingWarehouses = ["札幌倉庫", "東京倉庫", "埼玉倉庫", "横浜倉庫", "大阪倉庫", "神戸倉庫", "松山倉庫"] as const;
export const shippingCompanies = ["ヤマト運輸", "佐川急便", "福山通運", "西濃運輸", "チャーター", "その他輸送会社"] as const;
export const troubleCategories = ["荷役トラブル", "配送トラブル"] as const;
export const troubleDetailCategories = ["商品間違い", "数量過不足", "送付先間違い", "発送漏れ", "破損・汚損", "紛失", "その他の商品事故"] as const;
export const units = ["パレット", "ケース", "ボール", "ピース"] as const;

const getRandomItem = <T>(arr: readonly T[]): T => arr[Math.floor(Math.random() * arr.length)];
const getRandomDate = (start: Date, end: Date) => new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));

const createIncident = (id: number, occurrenceDate: Date, status: '2次情報調査中' | '3次情報調査中' | '完了' | '2次情報遅延' | '3次情報遅延'): Incident => {
    const creationDate = addDays(occurrenceDate, 1);
    const info1Date = creationDate;

    let incident: Incident = {
        id: id.toString(),
        creationDate: format(creationDate, 'yyyy-MM-dd'),
        organization: getRandomItem(organizations),
        creator: getRandomItem(creators),
        occurrenceDateTime: format(occurrenceDate, "yyyy-MM-dd'T'HH:mm"),
        occurrenceLocation: getRandomItem(occurrenceLocations),
        shippingWarehouse: getRandomItem(shippingWarehouses),
        shippingCompany: getRandomItem(shippingCompanies),
        troubleCategory: getRandomItem(troubleCategories),
        troubleDetailCategory: getRandomItem(troubleDetailCategories),
        details: `サンプル詳細 ${id}: ${getRandomItem(troubleDetailCategories)}が発生しました。`,
        voucherNumber: `V${10000 + id}`,
        customerCode: `CUST-${100 + id}`,
        productCode: `PROD-${200 + id}`,
        quantity: Math.floor(Math.random() * 100) + 1,
        unit: getRandomItem(units),
        infoInputDates: {
            info1: format(info1Date, 'yyyy-MM-dd'),
        },
        status: '2次情報調査中',
    };

    if (status === '2次情報遅延') {
        incident.infoInputDates!.info1 = format(addDays(new Date('2025-09-20'), -8), 'yyyy-MM-dd');
    }

    if (status === '3次情報調査中' || status === '完了' || status === '3次情報遅延') {
        const info2Date = addDays(info1Date, Math.floor(Math.random() * 6) + 1);
        incident = {
            ...incident,
            status: '3次情報調査中',
            inputDate: format(info2Date, 'yyyy-MM-dd'),
            processDescription: `発生経緯のサンプルテキスト ${id}。`,
            cause: `発生原因のサンプルテキスト ${id}。`,
            photoDataUri: "",
            infoInputDates: {
                ...incident.infoInputDates,
                info2: format(info2Date, 'yyyy-MM-dd'),
            },
        };
        if (status === '3次情報遅延') {
             incident.infoInputDates!.info2 = format(addDays(new Date('2025-09-20'), -8), 'yyyy-MM-dd');
        }
    }
    
    if (status === '完了') {
         const info3Date = addDays(new Date(incident.infoInputDates!.info2!), Math.floor(Math.random() * 6) + 1);
         incident = {
            ...incident,
            status: '完了',
            inputDate3: format(info3Date, 'yyyy-MM-dd'),
            recurrencePreventionMeasures: `再発防止策のサンプルテキスト ${id}。`,
            infoInputDates: {
                ...incident.infoInputDates,
                info3: format(info3Date, 'yyyy-MM-dd'),
            },
        };
    }

    // Set final status based on the input, as the logic above sets intermediate statuses.
    incident.status = status;
    return incident;
};


const createIncidentBatch = (startId: number, count: number, startDate: Date, endDate: Date) => {
    const incidents: Incident[] = [];
     for (let i = startId; i < startId + count; i++) {
        const occurrenceDate = getRandomDate(startDate, endDate);
        const statusOptions: Incident['status'][] = ['2次情報調査中', '3次情報調査中', '完了'];
        const status = statusOptions[Math.floor(Math.random() * statusOptions.length)];
        incidents.push(createIncident(i, occurrenceDate, status));
    }
    return incidents;
}


export const createInitialIncidents = (): Incident[] => {
    const sepStartDate = new Date('2025-09-01T00:00:00');
    const sepEndDate = new Date('2025-09-30T23:59:59');
    const sepIncidents = createIncidentBatch(1, 30, sepStartDate, sepEndDate);

    const augStartDate = new Date('2025-08-01T00:00:00');
    const augEndDate = new Date('2025-08-31T23:59:59');
    
    let augIncidents: Incident[] = [];
    let currentId = 31;

    const statusCounts = {
        '完了': 17,
        '3次情報遅延': 2,
        '2次情報遅延': 1,
    };
    
    for (const [status, count] of Object.entries(statusCounts)) {
        for (let i = 0; i < count; i++) {
            const occurrenceDate = getRandomDate(augStartDate, augEndDate);
            augIncidents.push(createIncident(currentId++, occurrenceDate, status as Incident['status']));
        }
    }
    
    return [...sepIncidents, ...augIncidents];
};

export const updateIncidentStatus = (incident: Incident): Incident => {
    const pseudoToday = new Date('2025-09-20'); 

    // Don't downgrade status if it's already delayed.
    if(incident.status === '2次情報遅延' || incident.status === '3次情報遅延') {
        return incident;
    }

    switch (incident.status) {
        case '2次情報調査中':
            if (incident.infoInputDates?.info1 && differenceInDays(pseudoToday, new Date(incident.infoInputDates.info1)) > 7) {
                return { ...incident, status: '2次情報遅延' };
            }
            break;
        case '3次情報調査中':
            if (incident.infoInputDates?.info2 && differenceInDays(pseudoToday, new Date(incident.infoInputDates.info2)) > 7) {
                return { ...incident, status: '3次情報遅延' };
            }
            break;
        default:
            break;
    }
    return incident;
  };
