"use client";

import type { Incident } from "@/lib/types";
import { ArrowUp, ArrowDown } from "lucide-react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "./ui/button";

type IncidentListProps = {
  incidents: Incident[];
  requestSort: (key: keyof Incident) => void;
  sortConfig: { key: keyof Incident; direction: string } | null;
  onEdit: (incident: Incident) => void;
};

export const getStatusBadgeVariant = (status?: Incident['status']) => {
  switch (status) {
    case '完了':
      return 'default';
    case '2次情報遅延':
    case '3次情報遅延':
      return 'destructive';
    default:
      return 'secondary';
  }
};


const SortableHeader = ({
  label,
  sortKey,
  requestSort,
  sortConfig,
}: {
  label: string;
  sortKey: keyof Incident;
  requestSort: (key: keyof Incident) => void;
  sortConfig: { key: keyof Incident; direction: string } | null;
}) => {
  const isSorted = sortConfig?.key === sortKey;
  const direction = isSorted ? sortConfig.direction : undefined;

  return (
    <TableHead>
      <Button variant="ghost" onClick={() => requestSort(sortKey)} className="-ml-4 h-8">
        {label}
        {isSorted && (direction === 'ascending' ? <ArrowUp className="ml-2 h-4 w-4" /> : <ArrowDown className="ml-2 h-4 w-4" />)}
      </Button>
    </TableHead>
  );
};


export function IncidentList({ incidents, requestSort, sortConfig, onEdit }: IncidentListProps) {
  const headers: { label: string; key: keyof Incident, className?: string }[] = [
    { label: "ステータス", key: "status" },
    { label: "発生日時", key: "occurrenceDateTime" },
    { label: "出荷元倉庫", key: "shippingWarehouse" },
    { label: "発生場所", key: "occurrenceLocation" },
    { label: "トラブル区分", key: "troubleCategory" },
    { label: "トラブル詳細区分", key: "troubleDetailCategory" },
    { label: "運送会社名", key: "shippingCompany" },
    { label: "作成者", key: "creator" },
  ];

  return (
    <>
      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              {headers.map((header) => (
                <SortableHeader
                  key={header.key}
                  label={header.label}
                  sortKey={header.key}
                  requestSort={requestSort}
                  sortConfig={sortConfig}
                />
              ))}
              <TableHead className="text-right w-[120px]">アクション</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {incidents.length > 0 ? (
              incidents.map((incident) => (
                <TableRow key={incident.id} onClick={() => onEdit(incident)} className="cursor-pointer">
                  <TableCell>
                    <Badge variant={getStatusBadgeVariant(incident.status)}>
                      {incident.status}
                    </Badge>
                  </TableCell>
                  <TableCell>{incident.occurrenceDateTime.replace('T', ' ')}</TableCell>
                  <TableCell>{incident.shippingWarehouse}</TableCell>
                  <TableCell>{incident.occurrenceLocation}</TableCell>
                  <TableCell>{incident.troubleCategory}</TableCell>
                  <TableCell>
                    <Badge variant="outline">{incident.troubleDetailCategory}</Badge>
                  </TableCell>
                  <TableCell>{incident.shippingCompany}</TableCell>
                  <TableCell>{incident.creator}</TableCell>
                  <TableCell className="text-right">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={(e) => { e.stopPropagation(); onEdit(incident); }}
                    >
                      表示・更新
                    </Button>
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={headers.length + 1} className="text-center h-24">
                  一致する物流トラブルはありません。
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </>
  );
}
