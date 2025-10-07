import { cn } from "@/lib/utils";
import type { SVGProps } from "react";

export function Logo(props: SVGProps<SVGSVGElement>) {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      {...props}
    >
      <path d="M14.4 9.5 12.5 6.5 14.4 9.5z" />
      <path d="M8.5 6.5 10.5 9.5 8.5 6.5z" />
      <path d="m18 12.5-1.9-5.1c-.1-.4-.4-.7-.8-.7H7.8c-.4 0-.7.3-.8.7L5 12.5" />
      <path d="M21 13c-2 0-2 2-4 2s-2-2-4-2-2 2-4 2-2-2-4-2" />
      <path d="M4 17.5c2 0 2-2 4-2s2 2 4 2 2-2 4 2 2 2 4 2" />
      <path d="M4 13v4.5" />
      <path d="M21 13v4.5" />
    </svg>
  );
}
